using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using YOTO;

public class NetCtrl
{
    private TcpClient tcpClient;
    private UdpClient udpClient;
    private const int HEADER_SIZE = 4;  // 消息头大小：uint32(4字节)
    private const int BUFFER_SIZE = 8192;
    private const int MAX_MESSAGE_SIZE = 65536; // 64KB
    private byte[] receiveBuffer;
    private int receiveOffset = 0;
    private volatile bool isDisposed = false;
    private volatile bool isTcpReceiving = false;

    public bool ConnectTcp(string ipAddress, int port)
    {
        try
        {
            isDisposed = false;
            tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(ipAddress), port);
            receiveBuffer = new byte[BUFFER_SIZE];
            Debug.Log("TCP connected to " + ipAddress + ":" + port);
            return true;
        }
        catch (Exception e)
        {
            tcpClient = null;
            Debug.Log("网络连接失败！");
            return false;
        }
    }

    public void SendTcpMessage(byte[] data)
    {
        if (tcpClient == null || !tcpClient.Connected) 
        {
            Debug.LogError("TCP client not connected or connection lost.");
            throw new InvalidOperationException("TCP client not connected.");
        }
        if (data == null)
        {
            Debug.LogError("Cannot send null message");
            return;
        }

        try
        {
            var stream = tcpClient.GetStream();
            if (!stream.CanWrite)
            {
                Debug.LogError("TCP stream is not writable");
                throw new InvalidOperationException("TCP stream is not writable");
            }

            // 解析消息类型（前4个字节）
            int messageType = 0;
            if (data.Length >= 4)
            {
                messageType = BitConverter.ToInt32(data, 0);
            }

            // 计算消息内容的长度
            uint contentLength = (uint)(data.Length);
            if (contentLength > MAX_MESSAGE_SIZE)
            {
                throw new InvalidOperationException($"Message size {contentLength} exceeds maximum allowed size {MAX_MESSAGE_SIZE}");
            }

            // 输出发送前的包体信息
            Debug.Log($"Sending packet - Total Size: {HEADER_SIZE + contentLength} bytes (Header: {HEADER_SIZE} bytes, Body: {contentLength} bytes)");
            if (data.Length >= 4)
            {
                Debug.Log($"Message Type: {(MessageType)messageType}, First 4 bytes: {BitConverter.ToString(data, 0, Math.Min(4, data.Length))}");
            }

            byte[] buffer = new byte[HEADER_SIZE + data.Length];

            // 1. 构造消息头 (uint32, 4字节, 小端序)
            byte[] lengthBytes = BitConverter.GetBytes(contentLength);

            // 2. 组装完整消息
            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, HEADER_SIZE);     // 复制消息长度
            Buffer.BlockCopy(data, 0, buffer, HEADER_SIZE, data.Length);  // 复制消息内容（包含类型和数据）

            // 3. 一次性发送完整消息
            stream.Write(buffer, 0, buffer.Length);
            
            Debug.Log($"Sent message: Size={contentLength} bytes, Type={messageType}, Header(hex)={BitConverter.ToString(lengthBytes)}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send message: {ex.Message}");
            throw;
        }
    }

    public void ReceiveTcpMessageAsync(Action<byte[]> callback)
    {
        if (tcpClient == null || isDisposed) return;

        try
        {
            var stream = tcpClient.GetStream();
            if (!stream.CanRead) return;

            isTcpReceiving = true;
            stream.BeginRead(receiveBuffer, receiveOffset, BUFFER_SIZE - receiveOffset, ar =>
            {
                if (isDisposed || !isTcpReceiving)
                {
                    return;
                }

                try
                {
                    var localStream = tcpClient?.GetStream();
                    if (localStream == null) return;

                    int bytesRead = localStream.EndRead(ar);
                    if (bytesRead <= 0)
                    {
                        Debug.LogWarning("Connection closed by server");
                        return;
                    }

                    receiveOffset += bytesRead;
                    ProcessReceiveBuffer(callback);

                    // 继续接收数据
                    if (!isDisposed && isTcpReceiving)
                    {
                        ReceiveTcpMessageAsync(callback);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 忽略已释放对象的异常
                    return;
                }
                catch (Exception ex)
                {
                    if (!isDisposed)
                    {
                        Debug.LogError($"Error receiving data: {ex.Message}");
                    }
                }
            }, null);
        }
        catch (Exception ex)
        {
            if (!isDisposed)
            {
                Debug.LogError($"Error starting receive operation: {ex.Message}");
            }
        }
    }

    private void ProcessReceiveBuffer(Action<byte[]> callback)
    {
        while (receiveOffset >= HEADER_SIZE)  // 确保至少有一个完整的消息头（4字节）
        {
            try
            {
                // 1. 解析消息头（4字节消息长度，小端序）
                byte[] lengthBytes = new byte[4];
                Array.Copy(receiveBuffer, 0, lengthBytes, 0, 4);
                uint messageSize = BitConverter.ToUInt32(lengthBytes, 0);

                // 2. 验证消息大小
                if (messageSize == 0 || messageSize > MAX_MESSAGE_SIZE)
                {
                    Debug.LogError($"无效的消息大小: {messageSize}, 限制: {MAX_MESSAGE_SIZE}字节");
                    receiveOffset = 0;
                    return;
                }

                // 3. 检查是否收到完整消息
                int totalSize = HEADER_SIZE + (int)messageSize;
                if (receiveOffset < totalSize)
                {
                    return;  // 等待更多数据
                }

                // 4. 提取完整消息（包括消息类型和内容）
                byte[] completeMessage = new byte[messageSize];
                Array.Copy(receiveBuffer, HEADER_SIZE, completeMessage, 0, (int)messageSize);

                // 5. 移动剩余数据到缓冲区开始位置
                if (receiveOffset > totalSize)
                {
                    Array.Copy(receiveBuffer, totalSize, receiveBuffer, 0, receiveOffset - totalSize);
                }
                receiveOffset -= totalSize;

                // 6. 回调处理消息
                callback(completeMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理消息缓冲区错误: {ex.Message}");
                if (receiveOffset > 0)
                {
                    string bufferHex = BitConverter.ToString(receiveBuffer, 0, Math.Min(receiveOffset, 32));
                    Debug.LogError($"当前缓冲区内容(前32字节): {bufferHex}");
                }
                receiveOffset = 0;
                break;
            }
        }
    }

    public bool ConnectUdp(int port)
    {
        try
        {
            udpClient = new UdpClient(port);
            Debug.Log("UDP client listening on port " + port);
            return true; 
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public void SendUdpMessage(string ipAddress, int port, byte[] data)
    {
        if (udpClient == null) throw new InvalidOperationException("UDP client not initialized.");
        udpClient.Send(data, data.Length, ipAddress, port);
    }

    public void ReceiveUdpMessageAsync(Action<byte[]> callback)
    {
        if (udpClient == null) throw new InvalidOperationException("UDP client not initialized.");
        udpClient.BeginReceive(ar =>
        {
            IPEndPoint remoteEP = null;
            byte[] data = udpClient.EndReceive(ar, ref remoteEP);
            callback(data);
        }, null);
    }

    public void Dispose()
    {
        try
        {
            isDisposed = true;
            isTcpReceiving = false;

            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    try
                    {
                        tcpClient.GetStream().Close();
                    }
                    catch { }
                }
                tcpClient.Close();
                tcpClient = null;
            }

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }

            receiveBuffer = null;
            receiveOffset = 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during NetCtrl disposal: {ex.Message}");
        }
    }
} 
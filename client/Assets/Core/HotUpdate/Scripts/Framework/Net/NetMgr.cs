using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
using Proto;
using System.Threading.Tasks;

namespace  YOTO
{

    public abstract class BaseResponse
    {
        public abstract void Init();
    }
    public class NetData
    {
        public MessageType type;
        public  IMessage message;
    }

    public enum NetDataType
    {
        Room,
        GamePlay
    }
    
    public class NetMgr 
    {
        private int UDPPort = 1234;
    public bool isLogin = false;
    private NetCtrl netCtrl = new NetCtrl();
    private Dictionary<MessageType, Action<IMessage>> messageHandlers = new Dictionary<MessageType, Action<IMessage>>();
    private  Queue<NetData> messageQueue = new Queue<NetData>();
    private  Dictionary<NetDataType, BaseResponse> netData = new Dictionary<NetDataType, BaseResponse>();
    private Dictionary<MessageType, MessageParser> messageParsers = new Dictionary<MessageType, MessageParser>();

    public void FixUpdate(float dt)
    {
        //切换到主线程执行
        if (messageQueue.Count > 0)
        {
            var msg =  messageQueue.Dequeue();
            HandleMessage(msg.type, msg.message);   
        }
        
        
    }
    public void Init()
    {
        YOTOFramework.Instance. StartCoroutine(InitCoroutine());
    }

    private IEnumerator InitCoroutine()
    {
#if UNITY_EDITOR
        UDPPort = 4321;
#endif
        const int maxRetries = 10;
        int currentRetry = 0;
        bool connected = false;
        while (currentRetry < maxRetries && !connected)
        {
            try
            {
                if (netCtrl.ConnectTcp("127.0.0.1", 8888) && netCtrl.ConnectUdp(UDPPort))
                {
                    netCtrl.ReceiveTcpMessageAsync(TCPCallBack);
                    netCtrl.ReceiveUdpMessageAsync(UDPCallBack);
                    InitHandlers();
                    connected = true;
                }
                else
                {
                    throw new Exception("Failed to establish connection");
                }
            }
            catch (Exception ex)
            {
                currentRetry++;
                if (currentRetry >= maxRetries)
                {
                    Debug.LogError($"Failed to initialize network after {maxRetries} attempts. Last error: {ex.Message}");
                    yield break;
                }
                Debug.LogWarning($"Failed to initialize network (attempt {currentRetry}/{maxRetries}): {ex.Message}. Retrying...");

            }
            yield return new WaitForSeconds(1); // 等待1秒后重试
        }
    }
    private void InitHandlers()
    {
    }

    public void SetResponseInstance(NetDataType type ,BaseResponse response)
    {
        response.Init();
        netData[type] = response;
    }

    public BaseResponse GetResponseInstance(NetDataType type)
    {
        if (netData.ContainsKey(type))
        {
            return netData[type];
        }
        else
        {
            return null;
        }
    }
    public void RegisterHandler(MessageType msgType, Action<IMessage> handler)
    {
        messageHandlers[msgType] = handler;
    }
    private void TCPCallBack(byte[] data)
    {
        if (data == null || data.Length < 4)
        {
            Debug.LogError("无效的消息数据");
            return;
        }

        try
        {
            // 前四个字节是消息类型（int）   
            int messageTypeValue = BitConverter.ToInt32(data, 0);
            MessageType messageType = (MessageType)messageTypeValue;
            
            // 处理心跳消息
            if (messageType == MessageType.HEARTBEAT_REQUEST)
            {
                Debug.Log("收到心跳请求");
                SendMessage(MessageType.HEARTBEAT_RESPONSE, null);
                return;
            }

            // 解析其他消息
            if (data.Length > 4)
            {
                byte[] messageData = new byte[data.Length - 4];
                Array.Copy(data, 4, messageData, 0, messageData.Length);
                try 
                {
                    IMessage message = ParseMessage(messageType, messageData);
                    if (message != null)
                    {
                        NetData netMsg = new NetData();
                        netMsg.message = message;
                        netMsg.type = messageType;
                        messageQueue.Enqueue(netMsg);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"解析消息错误:{messageType}， {ex.Message}, 数据长度: {data.Length}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"处理消息错误: {ex.Message}, 数据长度: {data.Length}");
        }
    }
    private void UDPCallBack(byte[] data)
    {
        if (data == null || data.Length < 4)
        {
            Debug.LogError("Invalid message data");
            return;
        }

        try
        {
            // 前四个字节是消息类型（int）   
            int messageTypeValue = BitConverter.ToInt32(data, 0);
            MessageType messageType = (MessageType)messageTypeValue;
        
            // Debug.Log($"Received UDP message type: {messageType}, length: {data.Length}");
        
            // 解析protobuf消息
            if (data.Length > 4)
            {
                byte[] messageData = new byte[data.Length - 4];
                Array.Copy(data, 4, messageData, 0, messageData.Length);
            
                // 直接解析protobuf消息
                IMessage message = ParseMessage(messageType, messageData);
                if (message != null)
                {
                    NetData netMsg = new NetData();
                    netMsg.message = message;
                    netMsg.type = messageType;
                    messageQueue.Enqueue(netMsg);
                    // Debug.Log($"Successfully parsed UDP message: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing message: {ex.Message}, Data length: {data.Length}, MessageType: {(MessageType)BitConverter.ToInt32(data, 0)}");
            string hexDump = BitConverter.ToString(data.Take(Math.Min(data.Length, 20)).ToArray());
            Debug.LogError($"Message data (first 20 bytes): {hexDump}");
        }

        // 继续接收UDP消息
        netCtrl.ReceiveUdpMessageAsync(UDPCallBack);
    }
    private IMessage ParseMessage(MessageType messageType, byte[] data)
    {
        // 心跳消息不需要解析
        if (messageType == MessageType.HEARTBEAT_REQUEST || messageType == MessageType.HEARTBEAT_RESPONSE)
        {
            return null;
        }

        if (messageParsers.TryGetValue(messageType, out var parser))
        {
            try 
            {
                return parser.ParseFrom(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析消息失败: {messageType}, 错误: {ex.Message}");
                return null;
            }
        }
        
        Debug.LogWarning($"未找到消息解析器: {messageType}");
        return null;
    }
    private void HandleMessage(MessageType messageType, IMessage message)
    {
        if (messageHandlers.TryGetValue(messageType, out var handler))
        {
            handler(message);
        }
        else
        {
            Debug.LogWarning($"No handler registered for message type: {messageType}");
        }
    }
    public void SendMessage(MessageType messageType, IMessage message)
    {
        const int maxRetries = 3;
        int currentRetry = 0;

        while (currentRetry < maxRetries)
        {
            try
            {
                if (message == null)
                {
                    // 发送空消息（只有消息类型，4字节）
                    byte[] messageTypeBytes = new byte[4]; // 使用4字节来匹配C++的enum大小
                    BitConverter.GetBytes((int)messageType).CopyTo(messageTypeBytes, 0);
                    netCtrl.SendTcpMessage(messageTypeBytes);
                }
                else
                {
                    // 发送完整消息（4字节消息类型 + 消息内容）
                    byte[] messageTypeBytes = new byte[4]; // 使用4字节来匹配C++的enum大小
                    BitConverter.GetBytes((int)messageType).CopyTo(messageTypeBytes, 0);
                    byte[] messageData = message.ToByteArray();
                    byte[] packageData = new byte[messageTypeBytes.Length + messageData.Length];
                    
                    messageTypeBytes.CopyTo(packageData, 0);
                    messageData.CopyTo(packageData, messageTypeBytes.Length);

                    netCtrl.SendTcpMessage(packageData);
                }
                // 发送成功，跳出循环
                break;
            }
            catch (Exception ex)
            {
                currentRetry++;
                if (currentRetry >= maxRetries)
                {
                    Debug.LogError($"Failed to send message after {maxRetries} attempts. Last error: {ex.Message}");
                    throw;
                }
                Debug.LogWarning($"Failed to send message (attempt {currentRetry}/{maxRetries}): {ex.Message}. Retrying...");
                System.Threading.Thread.Sleep(100); // 等待100ms后重试
            }
        }
    }
    public void SendMessageUDP(MessageType messageType, IMessage message)
    {
        if (message == null)
        {
            Debug.LogError("Cannot send null message via UDP");
            return;
        }

        try
        {
            // 序列化消息
            byte[] messageData = message.ToByteArray();
        
            // 创建包含消息类型的完整数据包
            byte[] packageData = new byte[4 + messageData.Length];
        
            // 写入消息类型
            BitConverter.GetBytes((int)messageType).CopyTo(packageData, 0);
        
            // 写入消息数据
            messageData.CopyTo(packageData, 4);

            // 发送数据
            netCtrl.SendUdpMessage("127.0.0.1", 8889, packageData);
        
            // Debug.Log($"Sent UDP message type: {messageType}, length: {packageData.Length}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending UDP message: {ex.Message}");
        }
    }
    public void Dispose()
    {
        try
        {
            netCtrl.Dispose();
            messageHandlers.Clear();
            messageQueue.Clear();
            netData.Clear();
            isLogin = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during NetMgr disposal: {ex.Message}");
        }
    }
    public void AddParser<T>(MessageType type) where T : IMessage<T>, new()
    {
        if (!messageParsers.ContainsKey(type))
        {
            messageParsers[type] = new MessageParser<T>(() => new T());
        }
    }
}
}

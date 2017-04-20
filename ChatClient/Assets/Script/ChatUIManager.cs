using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ProtoBuf;

public class ChatUIManager : MonoBehaviour {

    [SerializeField]
    private InputField nickInput;
    [SerializeField]
    private InputField chatInput;
    [SerializeField]
    private Button sendBtn;
    [SerializeField]
    private Text chatText;

    string nickContent;
    string chatContent;

    // Use this for initialization
    void Start () {
        NetClient.instance.Init();
        NetClient.instance.onRecMsg = OnRecMsg;
        chatText.text = "欢迎，可改昵称~";
	}
	
	// Update is called once per frame
	void Update () {
        NetClient.instance.ReceiveMsg();
	}

    public void OnSubmit()
    {
        nickContent = nickInput.text.Equals("") ? "Guest" : nickInput.text.Trim();
        chatContent = chatInput.text;

        NetClient.instance.SendMsg(serial(nickContent, chatContent));
    }

    //将二进制流解码成Protobuf对象，显示出来
    void OnRecMsg(byte[] msg)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(msg, 0, msg.Length);
            ms.Position = 0;
            ChatMsg chatMsg = Serializer.Deserialize<ChatMsg>(ms);
            chatText.text += "\n" + chatMsg.sender + ":" + chatMsg.msg;
            
        }
    }

    //将消息体编码成二进制流
    private byte[] serial(string sender, string msg)
    {
        ChatMsg chatMsg = new ChatMsg();
        chatMsg.sender = sender;
        chatMsg.msg = msg;

        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<ChatMsg>(ms, chatMsg);
            byte[] data = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(data, 0, data.Length);
            return data;
        }
    }
}

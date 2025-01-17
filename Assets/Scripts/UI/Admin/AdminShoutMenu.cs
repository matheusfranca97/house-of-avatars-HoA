using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class AdminShoutMenu : AdminMenu
{
    public InputField shoutInput;

    public override void Send()
    {
        ChatInput.instance.AddChatMessage(shoutInput.text, ChatMessageType.Shout);
        shoutInput.SetTextWithoutNotify("");
    }
}

using ProGaudi.MsgPack.Light;
using ProGaudi.Tarantool.Client;

namespace SocialNetwork.Dialog.DataAccess.Entities;

[MsgPackMap]
//[Space("")]
public class DialogMessage2
{
	[MsgPackMapElement("id")]
	public long Id { get; set; }
	[MsgPackMapElement("dialog_id")]
	public string DialogId { get; set; }
	[MsgPackMapElement("from_user_id")]
	public long From { get; set; }
	[MsgPackMapElement("to_user_id")]
	public long To { get; set; }
	[MsgPackMapElement("text")]
	public string Text { get; set; }
	[MsgPackMapElement("sent_at")]
	public string SentAt { get; set; }
}
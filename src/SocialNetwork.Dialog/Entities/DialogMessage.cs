﻿namespace SocialNetwork.Dialog.Entities;

public class DialogMessage
{
	public long MessageId { get; set; }
	public long From { get; set; }
	public long To { get; set; }
	public string Text { get; set; }
	public DateTime SentAt { get; set; }
	public bool IsRead { get; set; }
}
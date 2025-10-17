namespace Leauge_Auto_Accept.LCUTypes;

internal class LolChatConversationsMessagesV1
{
	public string type { get; set; }
	public string fromId { get; set; }
	public long fromSummonerId { get; set; }
	public bool isHistorical { get; set; }
	public string timestamp { get; set; }
	public string body { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

public record LolChatConversationsV1(
	[property: JsonPropertyName("gameName")] string GameName,
	[property: JsonPropertyName("gameTag")] string GameTag,
	[property: JsonPropertyName("id")] string Id,
	[property: JsonPropertyName("inviterId")] string InviterId,
	[property: JsonPropertyName("isMuted")] bool IsMuted,
	[property: JsonPropertyName("lastMessage")] object LastMessage,
	[property: JsonPropertyName("mucJwtDto")] MucJwtDto MucJwtDto,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("password")] string Password,
	[property: JsonPropertyName("pid")] string Pid,
	[property: JsonPropertyName("targetRegion")] string TargetRegion,
	[property: JsonPropertyName("type")] string Type,
	[property: JsonPropertyName("unreadMessageCount")] int UnreadMessageCount
);


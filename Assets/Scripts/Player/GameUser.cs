using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table("users")]
public class GameUser : BaseModel
{
    [PrimaryKey("uid", shouldInsert: true)]
    public string Id { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("auth_level", ignoreOnUpdate: true)]
    public int AuthLevel { get; set; }

    [Column("is_banned")]
    public bool IsBanned { get; set; } = false;

    [Column("is_kicked")]
    public bool IsKicked { get; set; } = false;

    [Column("kicked_minutes", nullValueHandling: NullValueHandling.Ignore)]
    public int? KickedMinutes { get; set; }

    [Column("kicked_timestamp", nullValueHandling: NullValueHandling.Ignore)]
    public DateTime? KickedTimestamp { get; set; }

    [Column("account_made_timestamp", ignoreOnUpdate: true)]
    public DateTime AccountMadeTimestamp { get; set; }

    public GameUser() { }

    public GameUser(string id, string username, DateTime accountMadeTimestamp)
    {
        Id = id;
        Username = username;
        AccountMadeTimestamp = accountMadeTimestamp;
    }

    public GameUser(string id, string username, int authLevel, bool isBanned, bool isKicked, int? kickedMinutes, DateTime? kickedTimestamp, DateTime accountMadeTimestamp)
    {
        Id = id;
        Username = username;
        AuthLevel = authLevel;
        IsBanned = isBanned;
        IsKicked = isKicked;
        KickedMinutes = kickedMinutes;
        KickedTimestamp = kickedTimestamp;
        AccountMadeTimestamp = accountMadeTimestamp;
    }
}

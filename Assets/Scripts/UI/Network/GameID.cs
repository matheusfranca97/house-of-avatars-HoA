using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table("game_ids")]
public class GameID : BaseModel
{
    [PrimaryKey("id", true)]
    public string Id { get; set; }

    [Column("join_code")]
    public string JoinCode { get; set; }

    public GameID() { }
    public GameID(string id, string joinCode)
    {
        Id = id;
        JoinCode = joinCode;
    }
}

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table("usernames")]
public class GameUsername : BaseModel
{
    [PrimaryKey("username", shouldInsert: true)]
    public string Username { get; set; }

    [Column("uid", ignoreOnUpdate: true)]
    public string Id { get; set; }

    public GameUsername() { }
    public GameUsername(string username, string id)
    {
        Username = username;
        Id = id;
    }
}
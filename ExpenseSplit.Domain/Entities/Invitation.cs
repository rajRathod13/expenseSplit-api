using ExpenseSplit.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpenseSplit.Domain.Entities;


public class Invitation : BaseClass
{
    [Key]
    public Guid InvitationId { get; set; }

    public Guid GroupId { get; set; }
    [JsonIgnore]
    public GroupDetail Group { get; set; }

    [ForeignKey(nameof(InviterUser))]
    public string InviterId { get; set; }
    [JsonIgnore]
    public User InviterUser { get; set; }

    [ForeignKey(nameof(InvitedUser))]
    public string InvitedUserId { get; set; }
    [JsonIgnore]
    public User InvitedUser { get; set; }

    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
}

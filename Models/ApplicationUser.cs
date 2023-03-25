namespace UserManagementWithIdentity.Models;


//> Inherit from IdentityUser to extend it.
public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; }

    //> Add(Upload) profile picture:
    public byte[] ProfilePicture { get; set; }
}

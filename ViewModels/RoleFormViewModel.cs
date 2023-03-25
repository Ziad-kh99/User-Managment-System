namespace UserManagementWithIdentity.ViewModels;

public class RoleFormViewModel
{
    [Required]
    [StringLength(256)]
    [Display(Name = "Role Name")]
    public string RoleName { get; set; } = string.Empty;
}
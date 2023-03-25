using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementWithIdentity.Migrations
{
    public partial class AssignAdminUserToAllRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO UserRoles (UserId, RoleId) SELECT '1c47f9ec-927e-4493-b38a-672b0b2c44db', Id FROM Roles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM UserRoles WHERE UserId = '1c47f9ec-927e-4493-b38a-672b0b2c44db'");
        }
    }
}

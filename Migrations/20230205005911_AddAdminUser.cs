using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementWithIdentity.Migrations
{
    public partial class AddAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Users([Id],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount],[FirstName],[LastName],[ProfilePicture]) VALUES('ca107c59-a2a0-4b94-8723-3c3d9326f856','admin1@asp.net','ADMIN1@ASP.NET','admin1@asp.net','ADMIN1@ASP.NET',0,'AQAAAAEAACcQAAAAEFzygBpSyiZWGprUqFKWfAMoQXGVp9T/zUyDelbcYFG1elWvOZ78G4sCw9zCCr4I7g==','RYAGQKJ4HSV74LJIMGVMITXIDGVV2O3X','ca72b27d-6031-495b-ba3a-5c81ebde803e',null,0,0,null,1,0,'Ziad','Admin',null)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users WHERE Id = 'ca107c59-a2a0-4b94-8723-3c3d9326f856'");
        }
    }
}

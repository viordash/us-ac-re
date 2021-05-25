using Microsoft.EntityFrameworkCore.Migrations;

namespace UsAcRe.Web.Server.Migrations {
	public partial class CreateDefaultRoles : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new string[] { "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { "Administrator", "Administrator", System.Guid.NewGuid().ToString() }
				);

			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new string[] { "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { "Tenant", "Tenant", System.Guid.NewGuid().ToString() }
				);

			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new string[] { "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { "User", "User", System.Guid.NewGuid().ToString() }
				);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Name",
				keyValue: "Administrator"
				);

			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Name",
				keyValue: "Tenant"
				);

			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Name",
				keyValue: "User"
				);
		}
	}
}

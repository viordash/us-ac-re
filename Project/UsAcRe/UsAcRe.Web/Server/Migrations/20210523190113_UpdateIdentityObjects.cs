using Microsoft.EntityFrameworkCore.Migrations;

namespace UsAcRe.Web.Server.Migrations {
	public partial class UpdateIdentityObjects : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "varchar(256)",
				maxLength: 256,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserTokens",
				type: "varchar(256)",
				maxLength: 256,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserTokens",
				type: "varchar(256)",
				maxLength: 256,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(100)");

			migrationBuilder.AlterColumn<string>(
				name: "ProviderKey",
				table: "AspNetUserLogins",
				type: "varchar(256)",
				maxLength: 256,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserLogins",
				type: "varchar(256)",
				maxLength: 256,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(128)",
				oldMaxLength: 128);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "varchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(256)",
				oldMaxLength: 256);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserTokens",
				type: "varchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(256)",
				oldMaxLength: 256);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserTokens",
				type: "varchar(100)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(256)",
				oldMaxLength: 256);

			migrationBuilder.AlterColumn<string>(
				name: "ProviderKey",
				table: "AspNetUserLogins",
				type: "varchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(256)",
				oldMaxLength: 256);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserLogins",
				type: "varchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "varchar(256)",
				oldMaxLength: 256);
		}
	}
}

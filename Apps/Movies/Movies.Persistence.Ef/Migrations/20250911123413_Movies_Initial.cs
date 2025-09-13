using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Ef.Migrations
{
    /// <inheritdoc />
    public partial class Movies_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                schema: "Mvs",
                table: "Movies");

            migrationBuilder.RenameTable(
                name: "Movies",
                schema: "Mvs",
                newName: "movies",
                newSchema: "Mvs");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "Mvs",
                table: "movies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReleaseYear",
                schema: "Mvs",
                table: "movies",
                newName: "release_year");

            migrationBuilder.RenameColumn(
                name: "MovieName",
                schema: "Mvs",
                table: "movies",
                newName: "movie_name");

            migrationBuilder.RenameColumn(
                name: "DirectorName",
                schema: "Mvs",
                table: "movies",
                newName: "director_name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_movies",
                schema: "Mvs",
                table: "movies",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_movies",
                schema: "Mvs",
                table: "movies");

            migrationBuilder.RenameTable(
                name: "movies",
                schema: "Mvs",
                newName: "Movies",
                newSchema: "Mvs");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "Mvs",
                table: "Movies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "release_year",
                schema: "Mvs",
                table: "Movies",
                newName: "ReleaseYear");

            migrationBuilder.RenameColumn(
                name: "movie_name",
                schema: "Mvs",
                table: "Movies",
                newName: "MovieName");

            migrationBuilder.RenameColumn(
                name: "director_name",
                schema: "Mvs",
                table: "Movies",
                newName: "DirectorName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                schema: "Mvs",
                table: "Movies",
                column: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    public partial class PopulaCategorias : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql(@"INSERT INTO ""Categorias"" (""Nome"", ""ImagemUrl"") VALUES ('Bebidas', 'bebidas.jpg');");
            mb.Sql(@"INSERT INTO ""Categorias"" (""Nome"", ""ImagemUrl"") VALUES ('Lanches', 'lanches.jpg');");
            mb.Sql(@"INSERT INTO ""Categorias"" (""Nome"", ""ImagemUrl"") VALUES ('Sobremesas', 'sobremesas.jpg');");
        }

        protected override void Down(MigrationBuilder mb)
        {
            // Deleta os dados e reseta a contagem da chave primária (SERIAL/IDENTITY)
            mb.Sql("TRUNCATE TABLE \"Categorias\" RESTART IDENTITY CASCADE;");
        }
    }
}
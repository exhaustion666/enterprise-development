using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable


namespace BeautySalon.Infrastructure.PostgreSql.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BeautyServices",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                Duration = table.Column<TimeSpan>(type: "interval", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BeautyServices", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LastName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                FirstName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Patronymic = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                Gender = table.Column<int>(type: "integer", nullable: false),
                Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Specialists",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PassportNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Specialization = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ExperienceYears = table.Column<int>(type: "integer", nullable: false),
                LastName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                FirstName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Patronymic = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                Gender = table.Column<int>(type: "integer", nullable: false),
                Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Specialists", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Bookings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                StartAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                SpecialistId = table.Column<int>(type: "integer", nullable: false),
                CustomerId = table.Column<int>(type: "integer", nullable: false),
                BeautyServiceId = table.Column<int>(type: "integer", nullable: false),
                IsRegularCustomer = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bookings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Bookings_BeautyServices_BeautyServiceId",
                    column: x => x.BeautyServiceId,
                    principalTable: "BeautyServices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Bookings_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Bookings_Specialists_SpecialistId",
                    column: x => x.SpecialistId,
                    principalTable: "Specialists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.InsertData(
            table: "BeautyServices",
            columns: ["Id", "Category", "Duration", "Name", "Price"],
            values: new object[,]
            {
                { 1, "Парикмахерские услуги", new TimeSpan(0, 1, 0, 0, 0), "Женская стрижка", 2500m },
                { 2, "Ногтевой сервис", new TimeSpan(0, 1, 30, 0, 0), "Маникюр с покрытием", 2000m },
                { 3, "Парикмахерские услуги", new TimeSpan(0, 0, 45, 0, 0), "Мужская стрижка", 1800m },
                { 4, "Парикмахерские услуги", new TimeSpan(0, 2, 30, 0, 0), "Окрашивание волос", 6000m },
                { 5, "Ногтевой сервис", new TimeSpan(0, 1, 30, 0, 0), "Педикюр", 2800m },
                { 6, "Визаж", new TimeSpan(0, 0, 30, 0, 0), "Оформление бровей", 1200m },
                { 7, "Косметология", new TimeSpan(0, 1, 0, 0, 0), "Чистка лица", 3500m },
                { 8, "СПА и массаж", new TimeSpan(0, 1, 0, 0, 0), "Расслабляющий массаж", 4000m },
                { 9, "Визаж", new TimeSpan(0, 1, 0, 0, 0), "Ламинирование ресниц", 2200m },
                { 10, "Визаж", new TimeSpan(0, 0, 45, 0, 0), "Дневной макияж", 2500m }
            });

        migrationBuilder.InsertData(
            table: "Customers",
            columns: ["Id", "DateOfBirth", "FirstName", "Gender", "LastName", "Patronymic", "Phone"],
            values: new object[,]
            {
                { 1, new DateOnly(1992, 3, 10), "Ольга", 1, "Смирнова", "Владимировна", "+79000000011" },
                { 2, new DateOnly(1985, 11, 25), "Игорь", 0, "Кузнецов", "Николаевич", "+79000000012" },
                { 3, new DateOnly(1998, 7, 19), "Елена", 1, "Васильева", null, "+79000000013" },
                { 4, new DateOnly(1995, 2, 14), "Мария", 1, "Соколова", "Сергеевна", "+79000000014" },
                { 5, new DateOnly(1990, 8, 11), "Андрей", 0, "Лебедев", "Васильевич", "+79000000015" },
                { 6, new DateOnly(1988, 12, 1), "Наталья", 1, "Козлова", "Игоревна", "+79000000016" },
                { 7, new DateOnly(1993, 5, 20), "Максим", 0, "Новиков", "Александрович", "+79000000017" },
                { 8, new DateOnly(1997, 9, 15), "Анна", 1, "Морозова", "Константиновна", "+79000000018" },
                { 9, new DateOnly(1982, 4, 3), "Сергей", 0, "Волков", "Дмитриевич", "+79000000019" },
                { 10, new DateOnly(1999, 11, 30), "Юлия", 1, "Белова", "Андреевна", "+79000000020" }
            });

        migrationBuilder.InsertData(
            table: "Specialists",
            columns: ["Id", "DateOfBirth", "ExperienceYears", "FirstName", "Gender", "LastName", "PassportNumber", "Patronymic", "Phone", "Specialization"],
            values: new object[,]
            {
                { 1, new DateOnly(1990, 5, 14), 8, "Анна", 1, "Иванова", "1111000001", "Сергеевна", "+79000000001", "Парикмахер-стилист" },
                { 2, new DateOnly(1995, 8, 22), 3, "Мария", 1, "Петрова", "1111000002", "Игоревна", "+79000000002", "Мастер ногтевого сервиса" },
                { 3, new DateOnly(1988, 12, 3), 6, "Роман", 0, "Мансуров", "1111000003", "Михайлович", "+79000000003", "Барбер" },
                { 4, new DateOnly(1992, 4, 12), 5, "Ольга", 1, "Ковалёва", "1111000004", "Николаевна", "+79000000004", "Косметолог" },
                { 5, new DateOnly(1987, 11, 5), 10, "Данила", 0, "Черкашин", "1111000005", "Станиславович", "+79000000005", "Массажист" },
                { 6, new DateOnly(1994, 1, 30), 4, "Екатерина", 1, "Васильева", "1111000006", "Дмитриевна", "+79000000006", "Стилист-колорист" },
                { 7, new DateOnly(1991, 9, 18), 7, "Артем", 0, "Соколов", "1111000007", "Михайлович", "+79000000007", "Барбер" },
                { 8, new DateOnly(1996, 6, 25), 3, "Наталья", 1, "Попова", "1111000008", "Сергеевна", "+79000000008", "Визажист" },
                { 9, new DateOnly(1989, 3, 14), 5, "Роман", 0, "Новиков", "1111000009", "Викторович", "+79000000009", "Мастер бровей" },
                { 10, new DateOnly(1993, 10, 8), 6, "Елена", 1, "Зайцева", "1111000010", "Васильевна", "+79000000010", "Мастер ногтевого сервиса" }
            });

        migrationBuilder.InsertData(
            table: "Bookings",
            columns: ["Id", "BeautyServiceId", "CustomerId", "IsRegularCustomer", "SpecialistId", "StartAt"],
            values: new object[,]
            {
                { 1, 1, 1, true, 1, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 2, 4, 1, true, 1, new DateTimeOffset(new DateTime(2026, 9, 1, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 3, 1, 3, false, 1, new DateTimeOffset(new DateTime(2026, 9, 1, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 4, 2, 3, false, 2, new DateTimeOffset(new DateTime(2026, 9, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 5, 5, 2, false, 2, new DateTimeOffset(new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 6, 3, 2, true, 3, new DateTimeOffset(new DateTime(2026, 9, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 7, 7, 4, true, 4, new DateTimeOffset(new DateTime(2026, 9, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 8, 8, 5, false, 5, new DateTimeOffset(new DateTime(2026, 9, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 9, 9, 8, true, 8, new DateTimeOffset(new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { 10, 2, 10, false, 10, new DateTimeOffset(new DateTime(2026, 9, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_BeautyServiceId",
            table: "Bookings",
            column: "BeautyServiceId");

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_CustomerId_StartAt",
            table: "Bookings",
            columns: ["CustomerId", "StartAt"]);

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_SpecialistId_StartAt",
            table: "Bookings",
            columns: ["SpecialistId", "StartAt"]);

        migrationBuilder.CreateIndex(
            name: "IX_Specialists_PassportNumber",
            table: "Specialists",
            column: "PassportNumber",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Bookings");

        migrationBuilder.DropTable(
            name: "BeautyServices");

        migrationBuilder.DropTable(
            name: "Customers");

        migrationBuilder.DropTable(
            name: "Specialists");
    }
}

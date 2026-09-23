using System;
using System.Collections.Generic;
using System.Text;
using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Enums;

namespace BeautySalon.Tests;

public class SalonFixture
{
    public List<Specialist> Specialists { get; } = [];
    public List<Customer> Customers { get; } = [];
    public List<BeautyService> BeautyServices { get; } = [];
    public List<Booking> Bookings { get; } = [];
    public SalonFixture()
    {
        var specialist1 = new Specialist
        {
            Id = 0,
            LastName = "Иванова",
            FirstName = "Анна",
            Patronymic = "Сергеевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1990, 5, 14),
            Phone = "+79000000001",
            PassportNumber = "1111000001",
            Specialization = "Парикмахер-стилист",
            ExperienceYears = 8
        };

        var specialist2 = new Specialist
        {
            Id = 1,
            LastName = "Петрова",
            FirstName = "Мария",
            Patronymic = "Игоревна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1995, 8, 22),
            Phone = "+79000000002",
            PassportNumber = "1111000002",
            Specialization = "Мастер ногтевого сервиса",
            ExperienceYears = 3
        };

        var specialist3 = new Specialist
        {
            Id = 2,
            LastName = "Мансуров",
            FirstName = "Роман",
            Patronymic = "Михайлович",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1988, 12, 3),
            Phone = "+79000000003",
            PassportNumber = "1111000003",
            Specialization = "Барбер",
            ExperienceYears = 6
        };

        var specialist4 = new Specialist
        {
            Id = 3,
            LastName = "Ковалёва",
            FirstName = "Ольга",
            Patronymic = "Николаевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1992, 4, 12),
            Phone = "+79000000004",
            PassportNumber = "1111000004",
            Specialization = "Косметолог",
            ExperienceYears = 5
        };

        var specialist5 = new Specialist
        {
            Id = 4,
            LastName = "Черкашин",
            FirstName = "Данила",
            Patronymic = "Станиславович",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1987, 11, 5),
            Phone = "+79000000005",
            PassportNumber = "1111000005",
            Specialization = "Массажист",
            ExperienceYears = 10
        };

        var specialist6 = new Specialist
        {
            Id = 5,
            LastName = "Васильева",
            FirstName = "Екатерина",
            Patronymic = "Дмитриевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1994, 1, 30),
            Phone = "+79000000006",
            PassportNumber = "1111000006",
            Specialization = "Стилист-колорист",
            ExperienceYears = 4
        };

        var specialist7 = new Specialist
        {
            Id = 6,
            LastName = "Соколов",
            FirstName = "Артем",
            Patronymic = "Михайлович",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1991, 9, 18),
            Phone = "+79000000007",
            PassportNumber = "1111000007",
            Specialization = "Барбер",
            ExperienceYears = 7
        };

        var specialist8 = new Specialist
        {
            Id = 7,
            LastName = "Попова",
            FirstName = "Наталья",
            Patronymic = "Сергеевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1996, 6, 25),
            Phone = "+79000000008",
            PassportNumber = "1111000008",
            Specialization = "Визажист",
            ExperienceYears = 3
        };

        var specialist9 = new Specialist
        {
            Id = 8,
            LastName = "Новиков",
            FirstName = "Роман",
            Patronymic = "Викторович",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1989, 3, 14),
            Phone = "+79000000009",
            PassportNumber = "1111000009",
            Specialization = "Мастер бровей",
            ExperienceYears = 5
        };

        var specialist10 = new Specialist
        {
            Id = 9,
            LastName = "Зайцева",
            FirstName = "Елена",
            Patronymic = "Васильевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1993, 10, 8),
            Phone = "+79000000010",
            PassportNumber = "1111000010",
            Specialization = "Мастер ногтевого сервиса",
            ExperienceYears = 6
        };

        Specialists.AddRange(
        [
        specialist1,
        specialist2,
        specialist3,
        specialist4,
        specialist5,
        specialist6,
        specialist7,
        specialist8,
        specialist9,
        specialist10
        ]);

        var customer1 = new Customer
        {
            Id = 0,
            LastName = "Смирнова",
            FirstName = "Ольга",
            Patronymic = "Владимировна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1992, 3, 10),
            Phone = "+79000000011"
        };

        var customer2 = new Customer
        {
            Id = 1,
            LastName = "Кузнецов",
            FirstName = "Игорь",
            Patronymic = "Николаевич",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1985, 11, 25),
            Phone = "+79000000012"
        };

        var customer3 = new Customer
        {
            Id = 2,
            LastName = "Васильева",
            FirstName = "Елена",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1998, 7, 19),
            Phone = "+79000000013"
        };

        var customer4 = new Customer
        {
            Id = 3,
            LastName = "Соколова",
            FirstName = "Мария",
            Patronymic = "Сергеевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1995, 2, 14),
            Phone = "+79000000014"
        };

        var customer5 = new Customer
        {
            Id = 4,
            LastName = "Лебедев",
            FirstName = "Андрей",
            Patronymic = "Васильевич",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 8, 11),
            Phone = "+79000000015"
        };

        var customer6 = new Customer
        {
            Id = 5,
            LastName = "Козлова",
            FirstName = "Наталья",
            Patronymic = "Игоревна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1988, 12, 1),
            Phone = "+79000000016"
        };

        var customer7 = new Customer
        {
            Id = 6,
            LastName = "Новиков",
            FirstName = "Максим",
            Patronymic = "Александрович",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1993, 5, 20),
            Phone = "+79000000017"
        };

        var customer8 = new Customer
        {
            Id = 7,
            LastName = "Морозова",
            FirstName = "Анна",
            Patronymic = "Константиновна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1997, 9, 15),
            Phone = "+79000000018"
        };

        var customer9 = new Customer
        {
            Id = 8,
            LastName = "Волков",
            FirstName = "Сергей",
            Patronymic = "Дмитриевич",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1982, 4, 3),
            Phone = "+79000000019"
        };

        var customer10 = new Customer
        {
            Id = 9,
            LastName = "Белова",
            FirstName = "Юлия",
            Patronymic = "Андреевна",
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1999, 11, 30),
            Phone = "+79000000020"
        };

        Customers.AddRange(
        [
        customer1,
        customer2,
        customer3,
        customer4,
        customer5,
        customer6,
        customer7,
        customer8,
        customer9,
        customer10
        ]);

        var service1 = new BeautyService
        {
            Id = 0,
            Name = "Женская стрижка",
            Category = "Парикмахерские услуги",
            Price = 2500m,
            Duration = TimeSpan.FromHours(1)
        };

        var service2 = new BeautyService
        {
            Id = 1,
            Name = "Маникюр с покрытием",
            Category = "Ногтевой сервис",
            Price = 2000m,
            Duration = TimeSpan.FromMinutes(90)
        };

        var service3 = new BeautyService
        {
            Id = 2,
            Name = "Мужская стрижка",
            Category = "Парикмахерские услуги",
            Price = 1800m,
            Duration = TimeSpan.FromMinutes(45)
        };

        var service4 = new BeautyService
        {
            Id = 3,
            Name = "Окрашивание волос",
            Category = "Парикмахерские услуги",
            Price = 6000m,
            Duration = TimeSpan.FromMinutes(150)
        };

        var service5 = new BeautyService
        {
            Id = 4,
            Name = "Педикюр",
            Category = "Ногтевой сервис",
            Price = 2800m,
            Duration = TimeSpan.FromMinutes(90)
        };

        var service6 = new BeautyService
        {
            Id = 5,
            Name = "Оформление бровей",
            Category = "Визаж",
            Price = 1200m,
            Duration = TimeSpan.FromMinutes(30)
        };

        var service7 = new BeautyService
        {
            Id = 6,
            Name = "Чистка лица",
            Category = "Косметология",
            Price = 3500m,
            Duration = TimeSpan.FromHours(1)
        };

        var service8 = new BeautyService
        {
            Id = 7,
            Name = "Расслабляющий массаж",
            Category = "СПА и массаж",
            Price = 4000m,
            Duration = TimeSpan.FromHours(1)
        };

        var service9 = new BeautyService
        {
            Id = 8,
            Name = "Ламинирование ресниц",
            Category = "Визаж",
            Price = 2200m,
            Duration = TimeSpan.FromHours(1)
        };

        var service10 = new BeautyService
        {
            Id = 9,
            Name = "Дневной макияж",
            Category = "Визаж",
            Price = 2500m,
            Duration = TimeSpan.FromMinutes(45)
        };

        BeautyServices.AddRange(
        [
        service1,
        service2,
        service3,
        service4,
        service5,
        service6,
        service7,
        service8,
        service9,
        service10
        ]);

        var anchorDate = new DateTimeOffset(
            2026,
            9,
            1,
            10,
            0,
            0,
            TimeSpan.Zero);

        Bookings.AddRange(
        [
            new Booking
        {
            Id = 0,
            StartAt = anchorDate,
            SpecialistId = specialist1.Id,
            Specialist = specialist1,
            CustomerId = customer1.Id,
            Customer = customer1,
            BeautyServiceId = service1.Id,
            BeautyService = service1,
            IsRegularCustomer = true
        },
        new Booking
        {
            Id = 1,
            StartAt = anchorDate.AddMinutes(90),
            SpecialistId = specialist1.Id,
            Specialist = specialist1,
            CustomerId = customer1.Id,
            Customer = customer1,
            BeautyServiceId = service4.Id,
            BeautyService = service4,
            IsRegularCustomer = true
        },
        new Booking
        {
            Id = 2,
            StartAt = anchorDate.AddHours(5),
            SpecialistId = specialist1.Id,
            Specialist = specialist1,
            CustomerId = customer3.Id,
            Customer = customer3,
            BeautyServiceId = service1.Id,
            BeautyService = service1,
            IsRegularCustomer = false
        },
        new Booking
        {
            Id = 3,
            StartAt = anchorDate.AddHours(2),
            SpecialistId = specialist2.Id,
            Specialist = specialist2,
            CustomerId = customer3.Id,
            Customer = customer3,
            BeautyServiceId = service2.Id,
            BeautyService = service2,
            IsRegularCustomer = false
        },
        new Booking
        {
            Id = 4,
            StartAt = anchorDate.AddDays(1).AddHours(2),
            SpecialistId = specialist2.Id,
            Specialist = specialist2,
            CustomerId = customer2.Id,
            Customer = customer2,
            BeautyServiceId = service5.Id,
            BeautyService = service5,
            IsRegularCustomer = false
        },
        new Booking
        {
            Id = 5,
            StartAt = anchorDate.AddHours(2),
            SpecialistId = specialist3.Id,
            Specialist = specialist3,
            CustomerId = customer2.Id,
            Customer = customer2,
            BeautyServiceId = service3.Id,
            BeautyService = service3,
            IsRegularCustomer = true
        },
        new Booking
        {
            Id = 6,
            StartAt = anchorDate.AddDays(3).AddHours(1),
            SpecialistId = specialist4.Id,
            Specialist = specialist4,
            CustomerId = customer4.Id,
            Customer = customer4,
            BeautyServiceId = service7.Id,
            BeautyService = service7,
            IsRegularCustomer = true
        },
        new Booking
        {
            Id = 7,
            StartAt = anchorDate.AddDays(4).AddHours(3),
            SpecialistId = specialist5.Id,
            Specialist = specialist5,
            CustomerId = customer5.Id,
            Customer = customer5,
            BeautyServiceId = service8.Id,
            BeautyService = service8,
            IsRegularCustomer = false
        },
        new Booking
        {
            Id = 8,
            StartAt = anchorDate.AddDays(5).AddHours(2),
            SpecialistId = specialist8.Id,
            Specialist = specialist8,
            CustomerId = customer8.Id,
            Customer = customer8,
            BeautyServiceId = service9.Id,
            BeautyService = service9,
            IsRegularCustomer = true
        },
        new Booking
        {
            Id = 9,
            StartAt = anchorDate.AddDays(6).AddHours(4),
            SpecialistId = specialist10.Id,
            Specialist = specialist10,
            CustomerId = customer10.Id,
            Customer = customer10,
            BeautyServiceId = service2.Id,
            BeautyService = service2,
            IsRegularCustomer = false
        }
        ]);
    }
}

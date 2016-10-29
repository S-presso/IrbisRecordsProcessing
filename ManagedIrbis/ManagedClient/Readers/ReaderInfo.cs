/* ReaderInfo.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient.Readers
{
    /// <summary>
    /// Информация о читателе.
    /// </summary>
    [Serializable]
    public sealed class ReaderInfo
    {
        #region Properties

        /// <summary>
        /// ФИО. Комбинируется из полей 10, 11 и 12.
        /// </summary>
        public string Fio { get; set; }

        /// <summary>
        /// Фамилия. Поле 10.
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Имя. Поле 11.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество. Поле 12.
        /// </summary>
        public string Patronym { get; set; }

        /// <summary>
        /// Дата рождения. Поле 21.
        /// </summary>
        public string Birthdate { get; set; }

        /// <summary>
        /// Номер читательского. Поле 30.
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// Пол. Поле 23.
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// Категория. Поле 50.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Домашний адрес. Поле 13.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Место работы. Поле 15.
        /// </summary>
        public string Work { get; set; }

        /// <summary>
        /// Образование. Поле 20.
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// Домашний телефон. Поле 17.
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        /// Дата записи. Поле 51.
        /// </summary>
        public string RegistrationDateString { get; set; }

        public DateTime RegistrationDate{get { return RegistrationDateString.ParseIrbisDate(); }}

        /// <summary>
        /// Дата перерегистрации. Поле 52.
        /// </summary>
        public RegistrationInfo[] Registrations;

        /// <summary>
        /// Дата последней перерегистрации.
        /// </summary>
        public DateTime LastRegistrationDate
        {
            get
            {
                if ((Registrations == null) || (Registrations.Length == 0))
                {
                    return DateTime.MinValue;
                }
                return Registrations.Last().Date;
            }
        }

        public string LastRegistrationPlace
        {
            get
            {
                if ((Registrations == null) || (Registrations.Length == 0))
                {
                    return null;
                }
                return Registrations.Last().Place;
            }
        }

        /// <summary>
        /// Разрешенные места получения литературы. Поле 56.
        /// </summary>
        public string EnabledPlaces { get; set; }

        /// <summary>
        /// Запрещенные места получения литературы. Поле 57.
        /// </summary>
        public string DisabledPlaces { get; set; }

        /// <summary>
        /// Право пользования библиотекой. Поле 29.
        /// </summary>
        public string Rights { get; set; }

        /// <summary>
        /// Примечания. Поле 33.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Фотография читателя. Поле 950.
        /// </summary>
        public string PhotoFile { get; set; }

        /// <summary>
        /// Информация о посещениях.
        /// </summary>
        public VisitInfo[] Visits;

        /// <summary>
        /// Возраст, годы
        /// </summary>
        public int Age
        {
            get
            {
                if (string.IsNullOrEmpty(Birthdate))
                {
                    return 0;
                }
                string yearText = Birthdate;
                if (yearText.Length > 4)
                {
                    yearText = yearText.Substring(1, 4);
                }
                int year;
                if (!int.TryParse(yearText, out year))
                {
                    return 0;
                }
                return DateTime.Today.Year - year;
            }
        }

        public string AgeCategory
        {
            get
            {
                int age = Age;
                if (age > 65) return "> 65";
                if (age >= 55) return "55-64";
                if (age >= 45) return "45-54";
                if (age >= 35) return "35-44";
                if (age >= 25) return "25-34";
                if (age >= 18) return "18-24";
                return "< 18";
            }
        }

        /// <summary>
        /// Произвольные данные, ассоциированные с читателем.
        /// </summary>
        public object UserData;

        public DateTime FirstVisitDate
        {
            get
            {
                if ((Visits == null) || (Visits.Length == 0))
                {
                    return DateTime.MinValue;
                }
                return Visits.First().DateGiven;
            }
        }

        public DateTime LastVisitDate
        {
            get
            {
                if ((Visits == null) || (Visits.Length == 0))
                {
                    return DateTime.MinValue;
                }
                return Visits.Last().DateGiven;
            }
        }

        public string LastVisitPlace
        {
            get
            {
                if ((Visits == null) || (Visits.Length == 0))
                {
                    return null;
                }
                return Visits.Last().Department;
            }
        }

        public string LastVisitResponsible
        {
            get
            {
                if ((Visits == null) || (Visits.Length == 0))
                {
                    return null;
                }
                return Visits.Last().Responsible;
            }
        }

        #endregion

        #region Public methods

        public static ReaderInfo Parse(IrbisRecord record)
        {
            ReaderInfo result = new ReaderInfo
                                    {
                                        FamilyName = record.FM("10"),
                                        FirstName = record.FM("11"),
                                        Patronym = record.FM("12"),
                                        Birthdate = record.FM("21"),
                                        Ticket = record.FM("30"),
                                        Sex = record.FM("23"),
                                        Category = record.FM("50"),
                                        Address = record.FM("13"),
                                        Work = record.FM("15"),
                                        Education = record.FM("20"),
                                        HomePhone = record.FM("17"),
                                        RegistrationDateString = record.FM("51"),
                                        Registrations = record.Fields
                                            .GetField("52")
                                            .Select(field=>RegistrationInfo.Parse(field))
                                            .ToArray(),
                                        EnabledPlaces = record.FM("56"),
                                        DisabledPlaces = record.FM("57"),
                                        Rights = record.FM("29"),
                                        Remarks = record.FM("33"),
                                        PhotoFile = record.FM("950"),
                                        Visits = record.Fields
                                            .GetField("40")
                                            .Select(field=>VisitInfo.Parse(field))
                                            .ToArray()
                                    };

            string fio = result.FamilyName;
            if (!string.IsNullOrEmpty(result.FirstName))
            {
                fio = fio + " " + result.FirstName;
            }
            if (!string.IsNullOrEmpty(result.Patronym))
            {
                fio = fio + " " + result.Patronym;
            }
            result.Fio = fio;

            return result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format
                (
                    "{0} - {1}", 
                    Ticket,
                    Fio
                );
        }

        #endregion
    }
}

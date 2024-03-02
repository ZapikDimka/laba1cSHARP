using System;
using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Represents a user with properties related to birthdate and zodiac signs.
    /// </summary>
    public class User : BaseBindable
    {
        private DateTime _birthDate;
        private int _age;
        private bool _isBirthdayToday = false;
        private ChineseZodiac _chineseZodiac;
        private WesternZodiac _westernZodiac;
        private string _zodiacInfo;

        public string ZodiacInfo
        {
            get => _zodiacInfo;
            set => UpdateProperty(ref _zodiacInfo, value, nameof(ZodiacInfo));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with the specified birthdate.
        /// </summary>
        /// <param name="birthDate">The birthdate of the user.</param>
        public User(DateTime birthDate)
        {
            BirthDate = birthDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with today's date as the birthdate.
        /// </summary>
        public User() : this(DateTime.Today)
        {
        }

        /// <summary>
        /// Gets or sets the birthdate of the user.
        /// </summary>
        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (UpdatePropertyValidated(ref _birthDate, value, ValidateBirthDate, nameof(BirthDate)))
                {
                    Age = CalculateAge(BirthDate);
                    IsBirthdayToday = CalculateIsBirthdayToday(BirthDate);
                    ChineseZodiac = BirthDate.GetChineseZodiacSign();
                    WesternZodiac = BirthDate.GetWesternZodiacSign();
                    OnPropertyChanged(nameof(FormattedAge));
                    UpdateZodiacInfo();
                }
            }
        }

        /// <summary>
        /// Gets the age of the user in years.
        /// </summary>
        public int Age
        {
            get => _age;
            private set => UpdateProperty(ref _age, value, nameof(Age));
        }

        /// <summary>
        /// Gets a value indicating whether today is the user's birthday.
        /// </summary>
        public bool IsBirthdayToday
        {
            get => _isBirthdayToday;
            private set => UpdateProperty(ref _isBirthdayToday, value, nameof(IsBirthdayToday));
        }

        /// <summary>
        /// Gets the Chinese zodiac sign of the user.
        /// </summary>
        public ChineseZodiac ChineseZodiac
        {
            get => _chineseZodiac;
            private set => UpdateProperty(ref _chineseZodiac, value, nameof(ChineseZodiac));
        }

        /// <summary>
        /// Gets the Western zodiac sign of the user.
        /// </summary>
        public WesternZodiac WesternZodiac
        {
            get => _westernZodiac;
            private set => UpdateProperty(ref _westernZodiac, value, nameof(WesternZodiac));
        }

        /// <summary>
        /// Gets the formatted age information as the total number of years, months, and days lived.
        /// </summary>
        public string FormattedAge
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;

                // Check if the birthday has occurred for the current age
                if (BirthDate.Date > today.AddYears(-age))
                {
                    age--;
                }

                var lastBirthday = BirthDate.AddYears(age);

                // Calculate the difference in days
                var days = (today - lastBirthday).Days;

                // Array with the lengths of each month
                var daysInMonth = new int[]
                {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
                };

                // Calculate the number of full months
                var months = 0;

                // Adjust for cases where the birthday hasn't occurred yet this year
                if (today.Month < BirthDate.Month || (today.Month == BirthDate.Month && today.Day < BirthDate.Day))
                {
                    months = 12 - (BirthDate.Month - today.Month);
                }
                else
                {
                    months = today.Month - BirthDate.Month;
                }

                // Adjust for cases where the birthday hasn't occurred yet this year
                if (today.Day < BirthDate.Day)
                {
                    months--;

                    // Calculate the remaining days in the last month
                    var lastMonth = today.AddMonths(-1);
                    var daysInLastMonth = daysInMonth[lastMonth.Month - 1];
                    days = daysInLastMonth - (BirthDate.Day - today.Day) + 1;
                }

                // Calculate the number of years
                var years = age;

                return $"{years} years,\n{months} months,\n{days} days";
            }
        }




        /// <summary>
        /// Validates the user's birthdate, adding errors if necessary.
        /// </summary>
        private void ValidateBirthDate()
        {
            ClearPropertyErrors(nameof(BirthDate));
            var age = CalculateAge(BirthDate);
            if (age < 18)
                AddPropertyError(nameof(BirthDate), "User age can't be younger than 18");
            if (age >= 135)
                AddPropertyError(nameof(BirthDate), "User age must be less than 135 years");
        }

        /// <summary>
        /// Calculates the age based on the provided birthdate.
        /// </summary>
        /// <param name="birthDate">The birthdate of the user.</param>
        /// <returns>The age of the user in years.</returns>
        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
            {
                // The birthday hasn't occurred yet this year
                age--;
            }

            return age;
        }

        /// <summary>
        /// Determines whether today is the user's birthday.
        /// </summary>
        /// <param name="birthDate">The birthdate of the user.</param>
        /// <returns>True if today is the user's birthday; otherwise, false.</returns>
        public static bool CalculateIsBirthdayToday(DateTime birthDate)
        {
            var today = DateTime.Today;

            // Check if the birthdate matches today's month and day
            if (birthDate.Month == today.Month && birthDate.Day == today.Day)
            {
                // Ensure the birthdate year is not in the future (accounting for leap years)
                if (birthDate.Year <= today.Year)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the ZodiacInfo property based on the current Western and Chinese zodiac signs.
        /// </summary>
        public void UpdateZodiacInfo()
        {
            ZodiacInfo = GetZodiacInfo(WesternZodiac.ToString(), ChineseZodiac.ToString());
        }

        /// <summary>
        /// Gets combined information about Western and Chinese zodiac signs.
        /// </summary>
        /// <param name="westernZodiac">The Western zodiac sign.</param>
        /// <param name="chineseZodiac">The Chinese zodiac sign.</param>
        /// <returns>Combined information about the zodiac signs.</returns>
        public static string GetZodiacInfo(string westernZodiac, string chineseZodiac)
        {
            // General information about the Western zodiac sign
            string westernInfo = ZodiacUtils.GetWesternZodiacInfo(westernZodiac);

            // General information about the Chinese zodiac sign
            string chineseInfo = ZodiacUtils.GetChineseZodiacInfo(chineseZodiac);

            // Return the combined information
            return $"{westernInfo}\n{chineseInfo}";
        }
    }
}

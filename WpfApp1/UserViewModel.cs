using System.ComponentModel;
using System.Linq;
using System.Windows; 

namespace WpfApp1
{
    /// <summary>
    /// Represents a view model for managing user data and interactions.
    /// </summary>
    public class UserViewModel : BaseBindable
    {
        private User _user;
        private BirthdayWindow _birthdayWindow;

        /// <summary>
        /// Initializes a new instance of the UserViewModel class.
        /// </summary>
        public UserViewModel()
        {
            User = new User();
        }

        /// <summary>
        /// Gets or sets the user object.
        /// </summary>
        public User User
        {
            get => _user;
            set
            {
                // Check and update the user object
                if (UpdateProperty(ref _user, value, nameof(User)))
                {
                    // Add an event handler for user property changes
                    User.PropertyChanged += OnUserPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Handles the birthday update logic.
        /// </summary>
        private void OnBirthdayUpdate()
        {
            // Check for errors in the BirthDate property
            if (User.GetPropertyErrors(nameof(User.BirthDate)) is var errors && errors != null)
            {
                // Display an error message if there are errors
                MessageBox.Show(errors.ElementAt(0), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Check if today is the user's birthday
                if (User.CalculateIsBirthdayToday(User.BirthDate))
                {
                    // Show a birthday greeting in a new window
                    ShowBirthdayWindow();
                }
                else
                {
                    // Update the zodiac sign information
                    User.UpdateZodiacInfo();
                }
            }
        }

        /// <summary>
        /// Shows a birthday greeting window.
        /// </summary>
        private void ShowBirthdayWindow()
        {
            // Create a new birthday greeting window
            _birthdayWindow = new BirthdayWindow();

            // Pass information for display to the birthday window
            var birthdayViewModel = new BirthdayViewModel(User);
            _birthdayWindow.DataContext = birthdayViewModel;

            // Display the birthday greeting window
            _birthdayWindow.Show();
        }

        /// <summary>
        /// Handles tasks before updating a property.
        /// </summary>
        /// <param name="propertyName">The name of the property being updated.</param>
        protected override void PrePropertyUpdated(string propertyName)
        {
            // Remove the PropertyChanged event handler before updating the User property
            if (propertyName == nameof(User))
            {
                User.PropertyChanged -= OnUserPropertyChanged;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event for user properties.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Event arguments containing information about the property change.</param>
        private void OnUserPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            // If the BirthDate property changes, call the update method
            if (args.PropertyName == nameof(User.BirthDate))
            {
                OnBirthdayUpdate();
                // Added update for zodiac sign information
            }
        }
    }
}

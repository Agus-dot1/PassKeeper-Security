# PassKeeper

**PassKeeper** is a simple yet powerful and secure password manager, built with C# and WPF, designed to help users protect their sensitive information without the need for creating an account. This is my first large-scale project, developed as part of my degree, where I applied my knowledge of encryption, secure storage, and modern UI design principles. With PassKeeper, users can easily store, encrypt, and manage passwords locally, using a master key for authentication. The app allows users to manually add their passwords or automatically generate strong ones for improved security, making it an essential tool for anyone looking to safeguard their digital life.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Technologies](#technologies)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Project Structure](#project-structure)

## Features

- **Master Key Authentication**: Secure access with a master key that encrypts and decrypts passwords.
- **Local Storage**: Passwords are stored locally, eliminating the need for online accounts.
- **AES Encryption**: Ensures stored data is secure using AES encryption.
- **Password Management**: Manually add passwords or use the built-in password generator.
- **MVVM Architecture**: Code is organized for clarity and scalability.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Agus-dot1/PassKeeper-Security.git
2. Open the solution in Visual Studio.
3. Restore NuGet packages to ensure all dependencies are available.
4. Build and run the application.

## Usage

1. **Launch PassKeeper** and enter your master key to unlock the application.
2. **Add a New Password**: Click on the '+' button on the password page to open the modal dialog and enter your password details.
3. **Generate a Password**: Use the password generator to create secure, random passwords.
5. **Store Passwords Securely**: Your passwords will be encrypted and stored locally, accessible only with the correct master key.

## Technologies

- **C#**: Core language for application development.
- **WPF**: UI framework for building a clean, responsive desktop interface.
- **MVVM Toolkit**: Implements the MVVM pattern for a well-structured and maintainable codebase.
- **WPF UI**: Provides predefined components that are very usefull.
- **AES Encryption**: Ensures password data is securely encrypted and stored locally.
## Roadmap

- **1.0.0** - Initial Version
  - Account-free password management
  - Basic encryption using AES and local storage
  - Manual and auto-generated password options
  
If you want to see more of detail of the versions you could go to [passkeeper/versionnotes](https://passkeeper-security.vercel.app/versionnotes)

- **Future Updates**
  - **Enhanced Encryption Options**: Provide users with more choices for encrypting passwords.
  - **Mobile Support**: Develop a mobile version of PassKeeper for cross-platform access (first i need to learn .NET MAUI).
  - **Database Integration**: Optional MySQL integration for centralized storage of encrypted data.
  - **Security Auditing Tools**: Add tools to check password strength and offer security tips.
  - **Additional Customization**: Improve UI customization and theming options.
 
To see more future updates you can go to [passkeeper/futurefeatures](https://passkeeper-security.vercel.app/futurefeatures)

 
## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch:
   ```bash
   git checkout -b feature/YourFeature
3. Make your changes and commit them:
   ```bash
   git commit -m 'Add YourFeature'
4. Push to the branch:
   ```bash
   git push origin feature/YourFeature
5. Open a pull request with a description of the changes you've made.
6. Wait for review and feedback from the project maintainers.

## License
License
This project is licensed under the MIT License. See the LICENSE file for details.
    
## Project Structure

```plaintext
PassKeeper/
├── Assets/                # Images or custom fonts.
├── Helpers/               # Reusable converters.
├── Models/                # Data models for storing password information.
├── Services/              # Backend services for encryption, storage, etc.
├── ViewModels/            # MVVM ViewModels for handling UI logic.
├── Views/                 # WPF Views for the application's UI components.
├── App.xaml               # Application entry point and settings.
└── AppSettings.cs         # Main application window layout.

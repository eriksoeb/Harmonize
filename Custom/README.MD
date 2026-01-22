# CustomConfig

## The following files must be in the same directory as the executables Harmonize.exe
This folder contains configuration files required by the application at runtime.

## Connection.txt holds the database connection string with server , username and password

## Menues.txt holds tha languages of the buttons, columns in the application


Use Trusted connection and avoid user name and password in clear text, and keep connection details in separate envirnomnet files

⚠️ **Important:**  
All files in this folder **must be located in the same directory as the executable**:


⚠️ **Notes:**  
The application will not start if Connection.txt is missing or inaccessible

Ensure the file encoding is plain text (UTF-8 recommended)

The whole organization may use the same executable and configurateion, when using a trusted connection 

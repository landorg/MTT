# Mettler Toledo UC retail scale tooling

A basic scale application with a product db.
This is based on the work of @Lg0enga. Thanks a million for your support.

![UC_test_tool](https://github.com/landorg/MTT/blob/main/assets/screen1.png)
![UC_test_tool](https://github.com/landorg/MTT/blob/main/assets/screen2.png)

## Setup
A small setup is required on a clean Windows version on a UC retail scale. The load cell can only be accessed via 'COM2' if the Kontron board is opened using 'jida.dll'. After opening, the board must be 'reopened' every 30 seconds to continue using the serial port on the Kontron board.

### Loadcell
Copy `jida.dll` to `C:/Windows/system32/`

### Printer
The programm Use LibUSB to communicate with the scale printer. Om gebruikt te kunnen maken van de printer kan je met Zadig de printer driver installeren om gebruikt te kunnen maken van LibUSB.

Download Zadig https://zadig.akeo.ie/

Run zadig and select **Options** -> **List All Devices**

![Zadig setup 1](https://github.com/Lg0enga/MettlerToledoUcTestTools/blob/main/assets/Zadig_Setup_01.png)

Select Device `Mettler UCP00001H432` (Label Printer)

![Zadig setup 2](https://github.com/Lg0enga/MettlerToledoUcTestTools/blob/main/assets/Zadig_Setup_02.png)


## Launch App
To use the test app .NET6 Desktop runtime is required. 
https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.27-windows-x86-installer

The programm in directory `TestApp` should work on the scale when setup is done.

> [!IMPORTANT]
> The app must be opened with administrator rights

![UC_test_tool](https://github.com/Lg0enga/MettlerToledoUcTestTools/blob/main/assets/UC_test_tool.png)

## Todo
- Printer settings
    - Darkness
    - Print speed
    - Receipt or Label
    - Offset
    - Calibrate printer 
- Scale info
- Scale setup
    - Monitor brightness


## Tested scales
- UC-HTT-M
- UC-CDDT-M
- UC-SOCT-M

## Documentation

[Jida](https://github.com/Lg0enga/MettlerToledoUcTestTools/blob/main/docs/jida32.pdf)

[Scale commands](https://github.com/Lg0enga/MettlerToledoUcTestTools/blob/main/docs/mmt.pdf)

## Support

If you need help, create an issue. I would like to help you!


```
$$\       $$$$$$\   $$$$$$\  $$$$$$$\  $$$$$$$\  $$$$$$\ $$\   $$\ 
$$ |     $$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\ \_$$  _|$$$\  $$ |
$$ |     $$ /  $$ |$$ /  $$ |$$ |  $$ |$$ |  $$ |  $$ |  $$$$\ $$ |
$$ |     $$ |  $$ |$$$$$$$$ |$$ |  $$ |$$$$$$$\ |  $$ |  $$ $$\$$ |
$$ |     $$ |  $$ |$$  __$$ |$$ |  $$ |$$  __$$\   $$ |  $$ \$$$$ |
$$ |     $$ |  $$ |$$ |  $$ |$$ |  $$ |$$ |  $$ |  $$ |  $$ |\$$$ |
$$$$$$$$\ $$$$$$  |$$ |  $$ |$$$$$$$  |$$$$$$$  |$$$$$$\ $$ | \$$ |
\________|\______/ \__|  \__|\_______/ \_______/ \______|\__|  \__|
```
<hr>

Loadbin is a simple utility made for [LibHydrix](https://github.com/azureiangh/libhydrix) that allows you to load a binary file into another file at a specific line. There are versions for both Windows and Linux.

## Uses
- Injecting binary data into a file (Image, Fonts, Text, etc.)
- Keeping file sizes small until compile time.

## Usage


In the file you're injecting the binary data into, add a line with the following format:
```c
#[LOADBIN] <from-file-name>
```
### Windows
Then run the following command:
```shell
.\loadbinary <from-file> <to-file>
```
### Linux
Then run the following command:
```bash
./loadbinary <from-file> <to-file>
```


# ImageFinder
## By Thad Donaghue - 3/09/2019

ImageFinder is a utility for helping a user locate images on the web for use in a PowerPoint presentation. The program gathers a title and body from the user and displays resulting images based on the query. The program makes use of the free SplashBase Image API (http://www.splashbase.co/api).

## Installation

1. Clone this repository.
2. Open the project in Visual Studio and Rebuild.
3. Press Ctrl + F5 to run the program.


## Usage

The program accepts a title input and body input. 

When you add text to the body input field, the text will appear with a purple background in the "Search Terms" section above the Results section. In the body, you can separate words using a whitespace character so they will show up as separate selectable entities in the "Search Terms" section. Left-Clicking on this text will make it bold and will include it in the image search. Right-Clicking the text will remove the bold styling and will remove the text from the image search. You can select and deselect any number of these entries. 

Any text added to the title input will be included in the image search.

All results will appear in a dropdown in the "Results" section. Selecting an item from the dropdown will make the image appear.

## License
[MIT](https://choosealicense.com/licenses/mit/)
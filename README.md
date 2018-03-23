# SpRestUtilities for C#
The Sharepoint rest api is very powerful but writing code that does something with it can be tricky and costly sometimes. Everytime i wanted to use it i had to try to remeber how to do this and that, looking through code i've written and of course googling. So i thought it would be great to have a class library which would do all the dirty work for me and this is what this project is basically all about. But I'm warning you in advance: This hasn't been fully tested! Use it on your own responsibility!

## Getting Started
After referencing this solution in your project, which can be basically anything, all you have to do is to initialize the SpRestUtilities and tell it where to operate (sp-site-url) and who you are (username and pw).

```c#
using SP_REST_UTILITY;

SpRestUtilities myUT = new SpRestUtilities();
myUT.SiteUrl = "https://yoursharepoint.com/yoursite";
myUT.Credentials = new NetworkCredential("USERNAME", "PASSWORD");
```

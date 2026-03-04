# Net2-Workhours

This project is a .NET 10.0 WPF application that calculates the working hours from a given csv file.
The csv file is give by the Paxton Access Net2 System, and contains the access logs of the users. The application reads the csv file, and calculates the working hours of each user, and displays the results in a DataGrid.
After that you can export the results to excel files.

# Features

- Setting the Working hours for each Day (Start time, End time and if it possible to start earlier what is the earliest time to start)
- Setting the Break time for each Day
- Setting the Overtime grace time
- If entry is later than the start time, it will be visualized in red
- If exit is earlier than the end time, it will be visualized in red
- For each user, is visualized seperately a datagrid with the daily entry and exit time, the total working hours, the total overtime hours and the total break time, grouped by month and year
- Export the results to excel files

The CSV file should have the following format:

```
Datum/Uhrzeit;Benutzer;Transponder-Nummer;Ort des Ereignisses;Ereignis;Details;;;;;;
```

# Disclaimer

*This project is a personal project. I have no affiliation with Paxton Access. Trademarks and logos are the property of their respective owners.*
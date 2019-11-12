BCSCannedODataServiceHost

Given that it can sometimes be a challenge to set up a datasource that is usable for BCS, the BCSCannedODataServiceHost was created.

To run:
- Make sure that the files in the BCSODataSource folder are all extracted to the same folder 
  on your hard drive.

- Run the bcscannedodataservicehost.exe.  This will load the service host and provide an endpoint 
  for BCS to communicate with.  Keep the service host command window open at all times during your 
  testing.

- Run the bcsinmemorylobviewer.exe.  This will populate an in-memory database that can be queried 
  by BCS. 

- Click the Register button.  This will register the data with the service host.

- Click the Populate Base Data button.  This will populate the database with data.
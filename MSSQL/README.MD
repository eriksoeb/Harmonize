
## Database Setup (Microsoft SQL Server)

To use this project, you must first create the **Harmonize** database in Microsoft SQL Server.  
You can do this in one of the following ways, depending on whether you want **test data** or a **clean schema only**.

---

### Option A: Restore Database (Includes Test Data)

This option restores a preconfigured database with tables, stored procedures, functions, and sample data.

1. Open **SQL Server Management Studio (SSMS)**
2. Right-click **Databases** → **Restore Database…**
3. Select **Device** and browse to the `Harmonize.bak` file
4. Set the target database name to **Harmonize**
5. Click **OK** to complete the restore

✔ Includes test/sample data  
✔ Recommended for development and testing

---

### Option B: Generate Database Schema (No Test Data)

This option creates the database structure only (tables, stored procedures, and functions), without inserting any sample data.

1. Open **SQL Server Management Studio (SSMS)**
2. Create a new empty database named **Harmonize**
3. Open the `Harmonize.sql` script file
4. Execute the script against the **Harmonize** database

✔ Clean database schema  
✔ Recommended for production or fresh environments

---

### Notes
- Ensure your SQL Server version is compatible with the provided files
- You may need sufficient permissions to restore databases or run scripts

import pyodbc
import datetime
import supporting_material.statements as sql

#constants for script config
CONST_DB_DRIVER = '{SQL Server}'
CONST_DB_SERVER = 'kevin-win-vm'
CONST_DB_NAME = 'AdventureWorks2019'
CONST_DB_TRUSTED = 'yes'

#constants for log config
CONST_LOG_TIME_FORMAT = '%Y-%m-%d %H:%M:%S.%f'
CONST_LOG_INFO = 'INFO'
CONST_LOG_WARN = 'WARNING'
CONST_LOG_ERR = 'ERROR'

#globals for DB usage
success = False
connection = None

#establishes DB connection
def connect(server, database):
    global success
    global connection
    
    print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Establishing connection...')
          
    try:
        connection = pyodbc.connect('DRIVER='+CONST_DB_DRIVER+';' +'SERVER='+server+';' +'DATABASE='+database+';' +'TRUSTED_CONNECTION='+CONST_DB_TRUSTED)
    except pyodbc.Error as e:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_ERR + ' > Connection failed... Refer to full message...')
        print(e)
        success = False
    else:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Connection successful...')
        success = True

#establishing DB connection
connect(CONST_DB_SERVER, CONST_DB_NAME)

#running statements
if success:
    print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Running statements...')
    db = connection.cursor()
    
    #Fact.Sales
    try:
        print(sql.FACT_SALES_CREATE)
        db.execute(sql.FACT_SALES_CREATE)        
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Statement run successfully...')
    except pyodbc.Error as ex:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_WARN + ' > Failed running statement... Refer to full message...')
        print(ex)

    try:        
        print(sql.FACT_SALES_INSERT)
        db.execute(sql.FACT_SALES_INSERT)        
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Statement run successfully...')
    except pyodbc.Error as ex:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_WARN + ' > Failed running statement... Refer to full message...')
        print(ex)

    #Dim.Customer
    try:        
        print(sql.DIM_CUSTOMER_CREATE)
        db.execute(sql.DIM_CUSTOMER_CREATE)          
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Statement run successfully...')
    except pyodbc.Error as ex:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_WARN + ' > Failed running statement... Refer to full message...')
        print(ex)

    try:        
        print(sql.DIM_CUSTOMER_INSERT)
        db.execute(sql.DIM_CUSTOMER_INSERT)        
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Statement run successfully...')
    except pyodbc.Error as ex:
        print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_WARN + ' > Failed running statement... Refer to full message...')
        print(ex)        
else:
    print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_WARN + ' > Running statements failed... Refer to full log...')

connection.close()
print(datetime.datetime.now().strftime(CONST_LOG_TIME_FORMAT) + ' > ' + CONST_LOG_INFO + ' > Script finished running... Refer to full log...')

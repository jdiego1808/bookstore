"""
    Download library pyodbc by command `pip install pyodbc`. Ensure you have already installed pip.
    Fill your info in 4 variables below.
    Test this code first by uncommenting line 29 and 53, then go into your database to check whether the first 3 books are imported.
    If they are, turn to books.json and delete 3 first books.
    Comment line 29 and 53. Run again. 
"""

import pyodbc
import json

server = '127.0.0.1' 
database = 'BooksDB' 
username = 'sa' 
password = 'JDiego1808!' 
cnxn = pyodbc.connect('DRIVER=FreeTDS;SERVER=localhost;DATABASE=BooksDB;UID=sa;PWD=JDiego1808!', autocomit=True)
cursor = cnxn.cursor()
cursor.execute("SELECT @@version;") 
row = cursor.fetchone() 
while row: 
    print(row[0])
    row = cursor.fetchone()


# f = open('books.json', 'r')
# data = json.load(f)
# count = 0
# for i in data:
#     # count+=1
#     if len(i['authors'])==0 or len(i['categories'])==0:
#         continue
#     print(i['_id'])
#     cursor.execute("""INSERT INTO books (id, title, isbn, pageCount, publishedDate, thumbnailUrl, description, status) VALUES (?,?,?,?,?,?,?,?,?,?)""",
#         i['_id']['$oid'], 
#         i['title'], 
#         i['isbn'], 
#         i['pageCount'], 
#         i['publishedDate'],
#         i['thumbnailUrl'],
#         i['shortDescription'],
#         i['status']
#     )
#     # cnxn.commit()

#     for author in i['authors']:
#         cursor.execute("""INSERT INTO authors (book_id, author_name) VALUES (?,?)""", i['_id']['$oid'], author)

#     for category in i['categories']:
#         cursor.execute("""INSERT INTO categories (book_id, category) VALUES (?,?)""", i['_id']['$oid'], category)
#     # cnxn.commit()

    # if count==3: break
    

cnxn.close()
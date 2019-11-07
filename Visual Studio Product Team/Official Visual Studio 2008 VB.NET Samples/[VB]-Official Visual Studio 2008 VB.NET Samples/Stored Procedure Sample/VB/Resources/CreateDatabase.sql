USE MASTER
IF EXISTS (
           SELECT * 
             FROM master..sysdatabases
            WHERE Name = 'StoredProceduresDemo'
           )
  DROP DATABASE StoredProceduresDemo
  CREATE DATABASE StoredProceduresDemo

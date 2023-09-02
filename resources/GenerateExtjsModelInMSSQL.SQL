
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE [dbo].[procGenerateExtJsModel]
GO
/*======================================================================

VERY CAREFUL AS ENTITY FRAMEWORK TENDS TO RENAME COLUMNS SO YOU KIND OF 
HAVE TO MAKE SURE THE JSON EXPORTS FROM THE CONTROLLER MATCH THE MODEL
BECAUSE FOR EXAMPLE THE DISTRICT OBJECT DISTRICT FIELDS BECOMES DISTRICT1
IN THE ENTITY MODEL.

Based on 
http://www.sencha.com/forum/showthread.php?205334-MSSQL-Stored-Procedure-to-generate-an-Ext-JS-Model

Changed to work with unlimited columns
Fixed comma placement
Removed unneccessary code

Usage:
exec [dbo].[procGenerateExtJsModel] @ObjectName = 'Courses'
======================================================================*/
CREATE PROCEDURE 
[dbo].[procGenerateExtJsModel] 
@ObjectName varchar(max)
AS 

DECLARE @name varchar(max), @type varchar(max)

DECLARE objCursor CURSOR
FOR
SELECT sc.name, st.name type FROM syscolumns sc INNER JOIN systypes st ON st.xusertype = sc.xusertype WHERE Id=OBJECT_ID(@ObjectName)

OPEN objCursor
FETCH NEXT FROM objCursor INTO @name, @type
DECLARE @extType varchar(max) -- ExtJS type
DECLARE @ctorParms varchar(max)
SET @ctorParms = ''

IF @@FETCH_STATUS <> 0
 BEGIN
 CLOSE objCursor
 DEALLOCATE objCursor
 PRINT 'Error... Please check passed parameter'
 RETURN
END

DECLARE @pk varchar(max) -- primary key
SELECT @pk = column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = @ObjectName

PRINT 'Ext.define(''' + @ObjectName + ''', {'
PRINT '    extend: ''Ext.data.Model'','
PRINT '    idProperty: ''' + @pk + ''','
PRINT '    fields: [' 

declare @line varchar(max)

WHILE @@FETCH_STATUS = 0
BEGIN

	SET @extType = CASE
		WHEN @type LIKE '%char%' OR @type LIKE '%text%' THEN 'string'
		WHEN @type IN ('decimal', 'numeric', 'real', 'float')            THEN 'float'
		WHEN @type LIKE '%money%'                       THEN 'float'
		WHEN @type = 'bit'                              THEN 'boolean'
		WHEN @type LIKE '%int%'                         THEN 'int'
		WHEN @type LIKE '%uniqueidentifier%'            THEN 'string'
		WHEN @type LIKE '%datetime%' THEN 'date'
		ELSE 'auto' 
	END

	set @line = '        { name: ''' + @name + ''', type: ''' + @extType + ''' }'

	FETCH NEXT FROM objCursor
	INTO @name, @type

	if @@FETCH_STATUS = 0
	begin
		set @line = @line + ','
	end 
	print @line
END

PRINT '    ]'
PRINT '});'
PRINT ''
PRINT ''


CLOSE objCursor
DEALLOCATE objCursor
DECLARE @Result NVARCHAR(MAX) = 'window.routes = {' + CHAR(13) + CHAR(10)

DECLARE @Controller NVARCHAR(100)
DECLARE @Routes NVARCHAR(MAX)

DECLARE controller_cursor CURSOR FOR
SELECT DISTINCT ControllerName
FROM Menus
WHERE IsActive = 1 AND IsDeleted = 0 AND ActionName IS NOT NULL

OPEN controller_cursor
FETCH NEXT FROM controller_cursor INTO @Controller

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Routes = CHAR(13) + '    ' + LOWER(@Controller) + ': {' + CHAR(13)

    SELECT @Routes = @Routes +
        '        ' + LOWER(ControllerName) + '_' + LOWER(ActionName) + ': ''' +
        ISNULL(NULLIF(Area, ''), '') +
        CASE WHEN Area IS NOT NULL THEN '/' ELSE '' END +
        --ControllerName + '/'
		+ ActionName + ''',' + CHAR(13)
    FROM Menus
    WHERE ControllerName = @Controller AND IsActive = 1 AND IsDeleted = 0 AND ActionName IS NOT NULL

    SET @Routes = LEFT(@Routes, LEN(@Routes) - 2) -- حذف کامای آخر
    SET @Routes = @Routes + CHAR(13) + '    },' + CHAR(13)

    SET @Result = @Result + @Routes

    FETCH NEXT FROM controller_cursor INTO @Controller
END

CLOSE controller_cursor
DEALLOCATE controller_cursor

SET @Result = LEFT(@Result, LEN(@Result) - 3) + CHAR(13) + '};'

-- خروجی نهایی
PRINT @Result

USE [SanjeshP_Mosahebe]
GO
/****** Object:  StoredProcedure [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox]
@CurrentUserId uniqueidentifier
,@GroupSelected_GroupId uniqueidentifier
as
Begin
	SELECT distinct(M.Id) AS DISTINCT_MenuID
	, M.Id
	, @CurrentUserId as UserId
	, M.MenuTitle
	, M.ParentId
	, M.PageCode
	, M.ShowMenu
	, M.CssClass
	, M.Icon
	, M.Link
	, M.Area
	, M.ControllerName
	, M.ActionName
	-- لیست منو‌هایی که کاربر جاری به صورت فردی و گروهی به آن دسترسی دارد
	, CONVERT(bit,(SELECT CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_User.IsDeleted = 0 THEN 1 WHEN AMG_User.IsDeleted = 1 THEN 0 WHEN AMG_User.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Group_Checkecd
	, ISNULL((SELECT COUNT(Id) 
			FROM dbo.Menus AS MP
			WHERE (ParentId = M.Id)), 0) AS IsParent
	-- لیست منوهایی که گروه انتخاب شده به آن به دسترسی دراد
	, CONVERT(bit,0) as Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_Group.IsDeleted = 0 THEN 1 WHEN AMG_Group.IsDeleted = 1 THEN 0 WHEN AMG_Group.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_Group.IsDeleted = 0 THEN 1 WHEN AMG_Group.IsDeleted = 1 THEN 0 WHEN AMG_Group.IsDeleted IS NULL THEN 0 END)) AS [disabled]
	FROM dbo.Menus AS M 
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM ON AM.MenuId=M.Id and AM.UserId=@CurrentUserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User ON AMG_User.MenuId=M.Id and AMG_User.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@CurrentUserId 
																							and UG.GroupId=@GroupSelected_GroupId
																							and UG.IsActive=1 
																							and UG.IsDeleted=0)
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_Group ON AMG_Group.MenuId=M.Id and AMG_Group.GroupId=@GroupSelected_GroupId
	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
	order by PageCode,ControllerName
--End
End

GO
/****** Object:  StoredProcedure [dbo].[Get_UserCurrentAccess]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATE proc [dbo].[Get_UserCurrentAccess]
--@CurrentUserId uniqueidentifier
--as
--Begin
--	SELECT distinct(M.Id) AS DISTINCT_MenuID
--	, M.Id
--	, @CurrentUserId as UserId
--	, M.MenuTitle
--	, M.ParentId
--	, M.PageCode
--	, M.ShowMenu
--	, M.CssClass
--	, M.Icon
--	, M.Link
--	, M.Area
--	, M.ControllerName
--	, M.ActionName
--	, CONVERT(bit,0) AS CurentUser_Person_Checkecd
--	, CONVERT(bit,0) AS CurentUser_Group_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END)) AS Person_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END)) AS [disabled]
--	, ISNULL((SELECT COUNT(Id) 
--				FROM dbo.Menus AS MP
--				WHERE (ParentId = M.Id)), 0) AS IsParent
--	FROM dbo.Menus AS M 
--	LEFT OUTER JOIN dbo.UserAccessMenus AS AM ON AM.MenuId=M.Id and AM.UserId=@CurrentUserId
--	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG ON AMG.MenuId=M.Id and AMG.GroupId in (select UG.GroupId 
--																							from GroupUsers as UG
--																							where UG.UserId=@CurrentUserId and UG.IsActive=1 and UG.IsDeleted=0) 
--	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
--	order by PageCode,ControllerName
--End
Create PROCEDURE [dbo].[Get_UserCurrentAccess]
    @CurrentUserId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT 
        M.Id AS DISTINCT_MenuID,
        M.Id,
        @CurrentUserId AS UserId,
        M.MenuTitle,
        M.ParentId,
        M.PageCode,
        M.ShowMenu,
        M.CssClass,
        M.Icon,
        M.Link,
        M.Area,
        M.ControllerName,
        M.ActionName,
        CONVERT(BIT, 0) AS CurentUser_Person_Checkecd,
        CONVERT(BIT, 0) AS CurentUser_Group_Checkecd,
        X.Person_Checkecd,
        X.Group_Checkecd,
        X.Group_Checkecd AS [disabled],
        ISNULL(
            (SELECT COUNT(1)
             FROM dbo.Menus AS MP
             WHERE MP.ParentId = M.Id), 0) AS IsParent
    FROM dbo.Menus AS M
    LEFT JOIN dbo.UserAccessMenus AS AM 
        ON AM.MenuId = M.Id AND AM.UserId = @CurrentUserId
    LEFT JOIN dbo.GroupAccessMenus AS AMG 
        ON AMG.MenuId = M.Id AND AMG.GroupId IN (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @CurrentUserId AND UG.IsActive = 1 AND UG.IsDeleted = 0
        )
    CROSS APPLY (
        SELECT
            CONVERT(BIT, CASE WHEN AM.IsDeleted = 0 THEN 1 ELSE 0 END) AS Person_Checkecd,
            CONVERT(BIT, CASE WHEN AMG.IsDeleted = 0 THEN 1 ELSE 0 END) AS Group_Checkecd
    ) AS X
    WHERE 
        M.IsDeleted = 0 AND 
        M.IsActive = 1 AND 
        (X.Person_Checkecd = 1 OR X.Group_Checkecd = 1)
    ORDER BY M.PageCode, M.ControllerName
END

GO
/****** Object:  StoredProcedure [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox]
@CurrentUserId uniqueidentifier
,@UserSelected_UserId uniqueidentifier
as
Begin
	SELECT distinct(M.Id) AS DISTINCT_MenuID
	, M.Id
	, @CurrentUserId as UserId
	, M.MenuTitle
	, M.ParentId
	, M.PageCode
	, M.ShowMenu
	, M.CssClass
	, M.Icon
	, M.Link
	, M.Area
	, M.ControllerName
	, M.ActionName
	-- لیست منو‌هایی که کاربر جاری به صورت فردی و گروهی به آن دسترسی دارد
	, CONVERT(bit,(SELECT CASE WHEN AM_Current.IsDeleted = 0 THEN 1 WHEN AM_Current.IsDeleted = 1 THEN 0 WHEN AM_Current.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_User.IsDeleted = 0 THEN 1 WHEN AMG_User.IsDeleted = 1 THEN 0 WHEN AMG_User.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Group_Checkecd
	, ISNULL((SELECT COUNT(Id) 
			FROM dbo.Menus AS MP
			WHERE (ParentId = M.Id)), 0) AS IsParent
	-- لیست منوهایی که کار انتخاب شده به آن به صورت فردی و گروهی دسترسی دراد
	, CONVERT(bit,(SELECT CASE WHEN AM_SelectedUser.IsDeleted = 0 THEN 1 WHEN AM_SelectedUser.IsDeleted = 1 THEN 0 WHEN AM_SelectedUser.IsDeleted IS NULL THEN 0 END)) AS Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1 WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0 WHEN AMG_UserSelected_Group.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1 WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0 WHEN AMG_UserSelected_Group.IsDeleted IS NULL THEN 0 END)) AS [disabled]

	FROM dbo.Menus AS M 
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM_Current ON AM_Current.MenuId=M.Id and AM_Current.UserId=@CurrentUserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User ON AMG_User.MenuId=M.Id and AMG_User.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@CurrentUserId and UG.IsActive=1 and UG.IsDeleted=0)
	
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM_SelectedUser ON AM_SelectedUser.MenuId=M.Id and AM_SelectedUser.UserId=@UserSelected_UserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_UserSelected_Group ON AMG_UserSelected_Group.MenuId=M.Id and AMG_UserSelected_Group.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@UserSelected_UserId and UG.IsActive=1 and UG.IsDeleted=0)
	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
	order by PageCode,ControllerName
End
Go





--CREATE VIEW [dbo].[View_UserMenubars]
--AS
--SELECT    M.Id, M.MenuTitle, M.ParentId, M.PageCode, M.ShowMenu, M.CssClass, M.Icon, M.Link,M.Area, M.ControllerName, M.ActionName, CONVERT(bit,
--                          (SELECT    CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END AS Expr1)) AS Person_Checkecd, CONVERT(bit,
--                          (SELECT    CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END AS Expr1)) AS Group_Checkecd, AM.UserId, ISNULL
--                          ((SELECT    COUNT(Id) AS Expr1
--                              FROM         dbo.Menus AS MP
--                              WHERE     (ParentId = M.Id)), 0) AS IsParent
--FROM         dbo.Menus AS M LEFT OUTER JOIN
--                      dbo.UserAccessMenus AS AM ON AM.MenuId = M.Id LEFT OUTER JOIN
--                      dbo.GroupAccessMenus AS AMG ON AMG.MenuId = M.Id
--WHERE     (M.IsDeleted = 0) AND (M.IsActive = 1)
--GO


GO
/****** Object:  StoredProcedure [dbo].[Get_StudentsFinalScores_EducationDocument]    Script Date: 10/18/2025 2:12:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[Calculator_StudentsFinalScores_EducationDocument]
    @StudentId INT,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -------------------------------------------------------------
    -- مرحله 1: محاسبه FinalItemScore برای هر آیتم
    -- فقط رکوردهای فعال و حذف‌نشده
    -------------------------------------------------------------
    ;WITH ItemScores AS (
        SELECT 
            SFSS.StudentId,
            FI.FormItemCategoryId,
            SFSS.FormItemId,
            FI.Title AS ItemTitle,
            CASE 
                WHEN SUM(ISNULL(SFSS.Score, 0)) > FI.AllMaxScore THEN FI.AllMaxScore
                ELSE SUM(ISNULL(SFSS.Score, 0))
            END AS FinalItemScore
        FROM StudentFormSubmissions AS SFSS
        INNER JOIN FormItems AS FI 
            ON FI.Id = SFSS.FormItemId
            AND FI.IsActive = 1 AND FI.IsDeleted = 0
        WHERE SFSS.StudentId = @StudentId
            AND SFSS.IsActive = 1 AND SFSS.IsDeleted = 0
        GROUP BY 
            SFSS.StudentId, 
            SFSS.FormItemId, 
            FI.AllMaxScore, 
            FI.Title, 
            FI.FormItemCategoryId
    ),
    -------------------------------------------------------------
    -- مرحله 2: رتبه‌بندی آیتم‌ها در هر دسته
    -------------------------------------------------------------
    RankedItems AS (
        SELECT 
            IS1.StudentId,
            IS1.FormItemCategoryId,
            IS1.FormItemId,
            IS1.ItemTitle,
            IS1.FinalItemScore,
            ROW_NUMBER() OVER (
                PARTITION BY IS1.StudentId, IS1.FormItemCategoryId 
                ORDER BY IS1.FinalItemScore DESC
            ) AS rn
        FROM ItemScores IS1
    ),
    -------------------------------------------------------------
    -- مرحله 3: الحاق مشخصات دسته (فقط دسته‌های فعال)
    -------------------------------------------------------------
    SelectedItems AS (
        SELECT 
            R.StudentId,
            R.FormItemCategoryId,
            R.FormItemId,
            R.FinalItemScore,
            R.rn,
            FIC.MaxCount,
            FIC.MaxScore
        FROM RankedItems R
        INNER JOIN FormItemCategories FIC 
            ON FIC.Id = R.FormItemCategoryId
            AND FIC.IsActive = 1 AND FIC.IsDeleted = 0
    ),
    -------------------------------------------------------------
    -- مرحله 4: جمع نمره هر دسته
    -------------------------------------------------------------
    CategorySums AS (
        SELECT
            SI.StudentId,
            SI.FormItemCategoryId,
            SUM(
                CASE 
                    WHEN SI.MaxCount = 0 THEN SI.FinalItemScore
                    WHEN SI.rn <= SI.MaxCount THEN SI.FinalItemScore
                    ELSE 0
                END
            ) AS CategorySum,
            MAX(SI.MaxScore) AS CategoryMaxScore
        FROM SelectedItems SI
        GROUP BY
            SI.StudentId,
            SI.FormItemCategoryId
    ),
    -------------------------------------------------------------
    -- مرحله 5: اعمال سقف MaxScore روی جمع دسته
    -------------------------------------------------------------
    FinalCategoryScores AS (
        SELECT
            CS.StudentId,
            CS.FormItemCategoryId,
            CASE 
                WHEN CS.CategorySum > CS.CategoryMaxScore THEN CS.CategoryMaxScore
                ELSE CS.CategorySum
            END AS FinalCategoryScore
        FROM CategorySums CS
    )
    -------------------------------------------------------------
    -- مرحله 6: جمع نهایی برای هر دانشجو
    -------------------------------------------------------------
    SELECT
        @StudentId AS StudentId,
        SUM(FCS.FinalCategoryScore) AS FinalTotalScore
    INTO #TempFinalScore
    FROM FinalCategoryScores FCS
    GROUP BY FCS.StudentId;

    -------------------------------------------------------------
    -- مرحله 7: بروزرسانی یا درج جدول EducationDocumentScores
    -- فقط رکوردهای فعال و حذف‌نشده
    -------------------------------------------------------------
    IF EXISTS (
        SELECT 1 
        FROM EducationDocumentScores 
        WHERE StudentId = @StudentId AND IsActive = 1 AND IsDeleted = 0
    )
    BEGIN
        UPDATE EDS
        SET FinalTotalScore = TFS.FinalTotalScore
        FROM EducationDocumentScores EDS
        INNER JOIN #TempFinalScore TFS ON EDS.StudentId = TFS.StudentId
        WHERE EDS.IsActive = 1 AND EDS.IsDeleted = 0;
    END
    ELSE
    BEGIN
        INSERT INTO EducationDocumentScores (
            StudentId, 
            FinalTotalScore,
            CreatedDate,
            IsActive,
            IsDeleted,
            CreatedBy
        )
        SELECT 
            StudentId, 
            FinalTotalScore,
            GETDATE(),
            1,
            0,
            @UserId
        FROM #TempFinalScore;
    END
	Select * from EducationDocumentScores
    -------------------------------------------------------------
    -- مرحله 8: پاکسازی جدول موقت
    -------------------------------------------------------------
    DROP TABLE #TempFinalScore;
END

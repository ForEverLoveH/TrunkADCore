
UPDATE DbPersonInfos SET FinalScore=-1,State=0,BeginTime='' WHERE Id='' 

SELECT Id FROM DbPersonInfos WHERE ProjectId='' AND GroupName=''

UPDATE DbGroupInfos SET State=0 WHERE ProjectId='' AND Name=''

UPDATE ResultInfos SET IsRemoved=1 WHERE PersonId=''


SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE State=1

UPDATE ColorGroupInfos SET State=0,perSonGroup='' WHERE Name='分组1'

SELECT SortId,RoundId,Result,State,CreateTime FROM ResultInfos WHERE PersonIdNumber='202110104'

SELECT SortId,RoundId,Result,State,CreateTime FROM ResultInfos WHERE PersonIdNumber='{personId}'

UPDATE ColorGroupInfos SET State=2 WHERE Name=''

INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) VALUES('2022-07-14 16:46:51', (SELECT MAX(SortId) + 1 FROM ResultInfos), 0, '2',0,'王墨轩','202110105',1,0,0)


DELETE FROM ResultInfos WHERE PersonId=


UPDATE DbPersonInfos SET BeginTime='',FinalScore=-1 WHERE Id=''

SELECT CreateTime FROM ResultInfos WHERE PersonId='439F01D3-C281-4CAD-A380-CC6A2C2AFECF' GROUP BY RoundId ORDER BY SortId ASC LIMIT 3

SELECT GroupName,Name,IdNumber,SchoolName,BeginTime,FinalScore FROM DbPersonInfos WHERE ProjectId='' and GroupName=''

UPDATE DbGroupInfos SET IsRemoved=1 WHERE ProjectId= AND Name=''
SELECT Name,Color,perSonGroup FROM ColorGroupInfos WHERE State=1


update ColorGroupInfos SET perSonGroup='',State=1 WHERE Id=

SELECT ChipNO,GroupName,ChipSort FROM ChipInfos WHERE ColorGroupId='21'


UPDATE ColorGroupInfos set State=0,perSonGroup='' WHERE State!=0 OR perSonGroup!=''

SELECT Id,IdNumber,Name, FROM DbPersonInfos WHERE ProjectId=1 AND GroupName='组ID-1'


INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) 
VALUES('{CreateTime}',SELECT MAX(SortId)+1 FROM ResultInfos,0,'{PersonId}',{0},'{PersonName}','{PersonIdNumber}',{RoundId},0,0)

SELECT MAX(SortId)+1 FROM ResultInfos

SELECT Name,Color FROM ColorGroupInfos WHERE State=0 

UPDATE SportProjectInfos SET TurnsNumber0=2,TurnsNumber1=1 WHERE Id='4B88FFCDD6152D1FBFF98174A3D8B3F0';

SELECT Name,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Id='';

SELECT Id,Type,RoundCount,BestScoreMode,TestMethod,FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='0712中长跑'

SELECT Name FROM DbGroupInfos WHERE ProjectId='6A775D9450CE0D0F27C7E15D276DC415'

SELECT GroupName,Name,IdNumber,SchoolName FROM DbPersonInfos WHERE ProjectId='' AND GroupName=''

UPDATE DbPersonInfos SET State=0 WHERE IdNumber='202110104' AND ProjectId='6A775D9450CE0D0F27C7E15D276DC415'


DELETE FROM ChipInfos
DELETE FROM ColorGroupInfos

INSERT INTO ColorGroupInfos(Name,State) VALUES('蓝色',0);
INSERT INTO ChipInfos(ChipLabel,ChipNO,ColorGroupId,ChipSort,GroupName) VALUES('','','','','');

select last_insert_rowid() from ColorGroupInfos LIMIT 1;

SELECT Id,Name,Color FROM ColorGroupInfos;

SELECT Id,Chiplabel,ChipNO,GroupName,ChipSort FROM ChipInfos WHERE ColorGroupId='5';

UPDATE ColorGroupInfos SET Color='' WHERE Id=;


SELECT Id,Chiplabel,ChipNO,GroupName,ChipSort FROM ChipInfos WHERE ColorGroupId='2';

SELECT Id,Chiplabel,ChipNO,GroupName,ChipSort FROM ChipInfos WHERE ColorGroupId='6';

DELETE FROM ColorGroupInfos WHERE Id=;
DELETE FROM ChipInfos WHERE ColorGroupId=;


SELECT hex(randomblob(16))




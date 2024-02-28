# GetWordList
--insert into words (WordW, StartLetter, EndLetter, Length) values ('home and dry', 'h', 'y', 12)
--insert into wordsmeaning (WordsId, MeaningChn, MeaningEng) values (202057, '成功达成目标', 'To have achieved victory or success in a contest or other activity, to to be certain to achieve it')
update WordsMeaning set MeaningEng='To have achieved victory or success in a contest or other activity, or to be certain to achieve it' where Id=1
--select * from words where WordW like 'home%'
select * from words where Id=202057
select * from wordsmeaning where WordsId=202057

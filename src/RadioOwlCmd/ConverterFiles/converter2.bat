@ECHO ***** Download mp4a
@REM cmd line youtube-dl: https://github.com/ytdl-org/youtube-dl
ConverterFiles\youtube-dl.exe --restrict-filenames --output %2 %1
@REM
@ECHO ***** Convert mp4a => mp3
@REM cmd line k ffmpeg: https://ffmpeg.org/ffmpeg.html
@REM -y (global) Overwrite output files without asking.
@REM "-loglevel info" - zbytecne moc informaci, lepsi se zda "-loglevel warning -stats", tj jen pripadne warningy, ale natvrdo tisknout progres bar
ConverterFiles\ffmpeg -loglevel warning -stats -y -i %2 -acodec libmp3lame -ab 192k %3
@ECHO ******************************************************
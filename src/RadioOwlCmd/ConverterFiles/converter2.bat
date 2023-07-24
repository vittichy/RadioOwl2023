@ECHO ************************** MP4 ***********************
ConverterFiles\youtube-dl.exe --restrict-filenames --output %2 %1
@ECHO ************************** MP3 ***********************
@REM -y (global) Overwrite output files without asking.
ConverterFiles\ffmpeg -loglevel info -y -i %2 -acodec libmp3lame -ab 192k %3
@ECHO ******************************************************
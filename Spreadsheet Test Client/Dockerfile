FROM mono
ADD . /src
WORKDIR /src
RUN mono nuget.exe restore "Test Client.sln"
RUN xbuild /p:Configuration=Release
CMD ["cd", "src/Spreadsheet Test Client/bin/Release"]

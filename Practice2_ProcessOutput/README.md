# Practice2_ProcessOutput — практика 2

Консольное приложение .NET 9: запускает `dotnet --info`, раздельно собирает стандартный вывод и поток ошибок, выводит PID, длительность, код завершения, stdout и stderr.

## Запуск

```powershell
dotnet run --project Practice2_ProcessOutput.csproj
```

Ожидаемый результат:

```text
PID: 12345
Длительность: 00:00:01.2345678
Exit code: 0
=== STDOUT ===
...вывод dotnet --info...

=== STDERR ===

```

У `dotnet --info` поток stderr обычно пуст — это нормально.

## Проверка stderr в Windows

Временно замените команду на `cmd.exe`:

```csharp
startInfo.FileName = "cmd.exe";
startInfo.ArgumentList.Add("/c");
startInfo.ArgumentList.Add("echo Сообщение в stdout & echo Ошибка в stderr 1>&2");
```

Ожидаемый результат:

```text
=== STDOUT ===
Сообщение в stdout

=== STDERR ===
Ошибка в stderr
```

Код завершения обычно `0`.

## Почему чтение начинается до ожидания

Оба вызова `ReadToEndAsync()` запускаются до `WaitForExitAsync()`. Если сначала дождаться завершения, а потом начать читать, процесс может заблокироваться на заполненном буфере stdout/stderr, и ожидание никогда не закончится.

## Почему нужен `WaitForExitAsync()`

`Process.ExitCode` имеет смысл только после завершения процесса. Если убрать ожидание, процесс может ещё работать, а обращение к `ExitCode` у незавершённого процесса бросает `InvalidOperationException`. `Win32Exception` этот случай не перехватывает, поэтому приложение завершится аварийно.

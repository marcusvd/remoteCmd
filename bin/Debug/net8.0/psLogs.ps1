# Define the log file path
$logPath = "$env:ProgramFiles(x86)\nostopti\attachments\ScriptsToExecute"

# Start logging
Start-Transcript -Path $logPath -Append

try {
    # Example commands in the script
    Write-Host "Starting script execution..."
    Get-Date
    Get-Process
    Write-Host "Script executed successfully!"
}
catch {
    # Capture errors and log them
    Write-Host "An error occurred: $_"
}
finally {
    # Stop logging
    Stop-Transcript
}
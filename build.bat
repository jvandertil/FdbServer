@echo off
powershell -noprofile -executionPolicy RemoteSigned -file "%~dp0\build\build.ps1"

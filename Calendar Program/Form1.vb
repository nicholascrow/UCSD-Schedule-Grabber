Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.IO

Public Class Form1
    Public Structure DaysOfWeek
        Dim Monday As String
        Dim Tuesday As String
        Dim Wednesday As String
        Dim Thursday As String
        Dim Friday As String
    End Structure
    Public Structure CalendarEvent
        Dim className As String
        Dim classDays As String
        Dim classTime As String
        Dim location As String
    End Structure

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Clear()
        find_classes(WebBrowser1.DocumentText)

    End Sub
    Sub find_classes(source As String)
        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VCALENDAR")
        sb.AppendLine("PRODID:-//Google Inc//Google Calendar 70.9054//EN")
        sb.AppendLine("VERSION:2.0")
        sb.AppendLine("CALSCALE:GREGORIAN")
        sb.AppendLine("METHOD:PUBLISH")
        sb.AppendLine("X-WR-CALNAME:Test Calendar")
        sb.AppendLine("X-WR-TIMEZONE:America/Los_Angeles")
        sb.AppendLine("X-WR-CALDESC:This should be the description")
        sb.AppendLine("BEGIN:VTIMEZONE")
        sb.AppendLine("TZID:America/Los_Angeles")
        sb.AppendLine("X-LIC-LOCATION:America/Los_Angeles")
        sb.AppendLine("BEGIN:DAYLIGHT")
        sb.AppendLine("TZOFFSETFROM:-0800")
        sb.AppendLine("TZOFFSETTO:-0700")
        sb.AppendLine("TZNAME:PDT")
        sb.AppendLine("DTSTART:19700308T020000")
        sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU")
        sb.AppendLine("END:DAYLIGHT")
        sb.AppendLine("BEGIN:STANDARD")
        sb.AppendLine("TZOFFSETFROM:-0700")
        sb.AppendLine("TZOFFSETTO:-0800")
        sb.AppendLine("TZNAME:PST")
        sb.AppendLine("DTSTART:19701101T020000")
        sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU")
        sb.AppendLine("END:STANDARD")
        sb.AppendLine("END:VTIMEZONE")

        TextBox1.AppendText(sb.ToString)
        Dim r3 As New Regex("<TD\salign=center>.*NAME=(show[0-9]*)\s.*<\/TD>\s*.*face=verdana>([0-9\:pa]*).*<\/TD>\s*.*face=verdana>([0-9\:pa]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z0-9\s&;\-_]*).*<\/TD>\s*.*face=verdana>([a-zA-Z0-9&;\s-]*).*<\/TD>\s*.*face=verdana>([a-zA-Z&;]*).*<\/TD>\s*.*face=verdana>([a-zA-Z0-9&;\s-]*).*<\/TD>\s*", RegexOptions.Multiline)
        Dim matches3 As MatchCollection = Regex.Matches(source, r3.ToString, RegexOptions.Multiline)

        For Each foundClass As Match In matches3
            Dim className As String = foundClass.Groups(11).Value
            Dim classTime As String = foundClass.Groups(2).Value & "-" & foundClass.Groups(3).Value
            Dim ClassesOnDays As New DaysOfWeek
            ClassesOnDays.Monday = foundClass.Groups(4).Value
            ClassesOnDays.Tuesday = foundClass.Groups(5).Value
            ClassesOnDays.Wednesday = foundClass.Groups(6).Value
            ClassesOnDays.Thursday = foundClass.Groups(7).Value
            ClassesOnDays.Friday = foundClass.Groups(8).Value


            Dim classDays As String = findDays(ClassesOnDays)
            Dim classLocation As String = foundClass.Groups(12).Value
            Dim my_evt As New CalendarEvent
            my_evt.location = classLocation
            my_evt.classTime = classTime
            my_evt.className = className
            my_evt.classDays = classDays
            append_events(my_evt)

        Next

        TextBox1.AppendText("END:VCALENDAR")
        File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics").Dispose()
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics", TextBox1.Text, False)
        MsgBox("Done! There is now a file on your desktop named my_schedule.ics. Please take that file and upload it to your google calendar.")

    End Sub
    Function findDays(daything As DaysOfWeek)
        Dim days As String = ""

        If daything.Monday.Contains("X") Then
            days = days & "MO "
        End If
        If daything.Tuesday.Contains("X") Then
            days = days & "TU "
        End If
        If daything.Wednesday.Contains("X") Then
            days = days & "WE "
        End If
        If daything.Thursday.Contains("X") Then
            days = days & "TH "
        End If
        If daything.Friday.Contains("X") Then
            days = days & "FR "
        End If

        Return days
    End Function
    Sub append_events(evt As CalendarEvent)


        Dim startTime As String = evt.classTime.Split("-")(0)

        '' If 
        If startTime.Contains("p") Then
            startTime.Replace("p", "")

            If Not startTime.Split(":")(0) = 12 Then
                startTime = (startTime.Split(":")(0) + 12) Mod 24
            End If
        ElseIf startTime.Contains("a") Then
            startTime.Replace("a", "")
            startTime = startTime.Split(":")(0)
            If Convert.ToInt32(startTime) < 10 Then startTime = "0" & startTime
        End If


        Dim endTime As String = evt.classTime.Split("-")(1)

        If endTime.Contains("p") Then
            endTime.Replace("p", "")
            If Not endTime.Split(":")(0) = 12 Then
                endTime = (endTime.Split(":")(0) + 12) Mod 24
            Else
                endTime = endTime.Split(":")(0)
            End If
        ElseIf endTime.Contains("a") Then
            endTime.Replace("a", "")
            endTime = endTime.Split(":")(0)
            If Convert.ToInt32(endTime) < 10 Then endTime = "0" & endTime
        End If


        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VEVENT")
        sb.AppendLine("DTSTART;TZID=America/Los_Angeles:20150105T" & (startTime).Split(":")(0) & evt.classTime.Split("-")(0).Replace("p", "").Split(":")(1).Replace("a", "") & "00")
        sb.AppendLine("DTEND;TZID=America/Los_Angeles:20150105T" & (endTime) & evt.classTime.Split("-")(1).Replace("p", "").Split(":")(1).Replace("a", "") & "00")
        sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=20150314T140000Z;BYDAY=" & evt.classDays.Replace(" ", ","))
        sb.AppendLine("DTSTAMP:20150105T060809Z")
        sb.AppendLine("UID:" & System.Guid.NewGuid.ToString() & "@google.com")
        sb.AppendLine("CREATED:20150105T060759Z")
        sb.AppendLine("DESCRIPTION:")
        sb.AppendLine("LAST-MODIFIED:20150117T060759Z")
        sb.AppendLine("LOCATION:" & evt.location)
        sb.AppendLine("SEQUENCE:0")
        sb.AppendLine("STATUS:CONFIRMED")
        sb.AppendLine("SUMMARY:" & evt.className)
        sb.AppendLine("TRANSP:OPAQUE")
        sb.AppendLine("END:VEVENT")
        TextBox1.AppendText(sb.ToString)

    End Sub

End Class

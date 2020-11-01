table 98990 "Performance Profiler"
{
    Caption = 'Performance Profiler';
    DataClassification = CustomerContent;
    LookupPageId = "Performance Profiler List";
    DrillDownPageId = "Performance Profiler List";

    fields
    {
        field(10; Code; Code[20])
        {
            Caption = 'Code';
            DataClassification = CustomerContent;
        }

        field(100; Description; Text[100])
        {
            Caption = 'Description';
            DataClassification = CustomerContent;
        }

        field(110; "No. of Lines"; Integer)
        {
            Caption = 'No. of Lines';
            BlankZero = true;
            Editable = false;
            FieldClass = FlowField;
            CalcFormula = count("Performance Profiler Line" where("Performance Profiler Code" = field(Code)));
        }

        field(200; "Session Id"; Integer)
        {
            Caption = 'Session ID';
            BlankZero = true;
            MinValue = -1;
            TableRelation = "Active Session"."Session ID";
            ValidateTableRelation = false;

            trigger OnLookup()
            var
                activeSession: Record "Active Session";
            begin
                if Page.RunModal(Page::"Concurrent Session List", activeSession) = Action::LookupOK then begin
                    "Session Id" := activeSession."Session ID";
                end;
            end;
        }

        field(210; Threshold; Integer)
        {
            Caption = 'Threshold';
            BlankZero = true;
            MinValue = 0;
        }
    }

    keys
    {
        key(PK; Code)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
        fieldgroup(DropDown; Code, Description)
        {
        }
    }

    trigger OnDelete()
    begin
        DeleteRelatedTables();
    end;

    local procedure DeleteRelatedTables()
    var
        performanceProfilerLine: Record "Performance Profiler Line";
    begin
        performanceProfilerLine.SetFilter("Performance Profiler Code", '%1', Code);
        performanceProfilerLine.DeleteAll();
    end;

    local procedure ConfirmOverwrite(tableCaption: Text; profilerCode: Code[20]): Boolean
    var
        xyDoesAlreadyExistOverwriteQst: Label '%1 %2 does already exist. Overwrite?', Comment = '%1 = Table Identifier; %2 = Record Identifier';
    begin
        exit(Confirm(xyDoesAlreadyExistOverwriteQst, false, tableCaption, profilerCode));
    end;

    procedure CopyToArchive(force: Boolean): Boolean
    begin
        exit(CopyToArchive(Code, force));
    end;

    procedure CopyToArchive(profilerCode: Code[20]; force: Boolean): Boolean
    var
        performanceProfiler: Record "Performance Profiler";
        performanceProfilerLine: Record "Performance Profiler Line";
        performanceProfilerArchive: Record "Performance Profiler Archive";
        performanceProfilerLineArchive: Record "Perf. Profiler Line Archive";
    begin
        performanceProfiler.SetRange(Code, profilerCode);

        if performanceProfiler.FindSet() then begin
            performanceProfilerArchive.SetRange(Code, performanceProfiler.Code);

            if not (performanceProfilerArchive.IsEmpty() or force) then begin
                if not ConfirmOverwrite(performanceProfilerArchive.TableCaption(), performanceProfiler.Code) then begin
                    exit(false);
                end;
            end;

            repeat
                performanceProfilerArchive.TransferFields(performanceProfiler, true);
                performanceProfilerArchive.Insert();

                performanceProfilerLine.SetRange("Performance Profiler Code", performanceProfiler.Code);
                performanceProfilerLine.SetAutoCalcFields("Full Statement");

                if performanceProfilerLine.FindSet() then begin
                    repeat
                        performanceProfilerLineArchive.TransferFields(performanceProfilerLine, true);
                        performanceProfilerLineArchive.Insert();
                    until performanceProfilerLine.Next() = 0;
                end;
            until performanceProfiler.Next() = 0;
        end;

        exit(not performanceProfilerArchive.IsEmpty());
    end;

    procedure CopyFromArchive(force: Boolean): Boolean
    var
        performanceProfilerArchive: Record "Performance Profiler Archive";
    begin
        if Page.RunModal(0, performanceProfilerArchive) = Action::LookupOK then begin
            exit(CopyFromArchive(performanceProfilerArchive.Code, force));
        end;

        exit(false);
    end;

    procedure CopyFromArchive(profilerCode: Code[20]; force: Boolean): Boolean
    var
        performanceProfiler: Record "Performance Profiler";
        performanceProfilerLine: Record "Performance Profiler Line";
        performanceProfilerArchive: Record "Performance Profiler Archive";
        performanceProfilerArchiveLine: Record "Perf. Profiler Line Archive";
    begin
        performanceProfilerArchive.SetRange(Code, profilerCode);

        if performanceProfilerArchive.FindSet() then begin
            performanceProfiler.SetRange(Code, performanceProfilerArchive.Code);

            if not (performanceProfiler.IsEmpty() or force) then begin
                if not ConfirmOverwrite(performanceProfiler.TableCaption(), performanceProfilerArchive.Code) then begin
                    exit(false);
                end;
            end;

            repeat
                performanceProfiler.TransferFields(performanceProfilerArchive, true);
                performanceProfiler.Insert();

                performanceProfilerArchiveLine.SetRange("Performance Profiler Code", performanceProfilerArchive.Code);
                performanceProfilerArchiveLine.SetAutoCalcFields("Full Statement");

                if performanceProfilerArchiveLine.FindSet() then begin
                    repeat
                        performanceProfilerLine.TransferFields(performanceProfilerArchiveLine, true);
                        performanceProfilerLine.Insert();
                    until performanceProfilerArchiveLine.Next() = 0;
                end;
            until performanceProfilerArchive.Next() = 0;
        end;

        exit(not performanceProfiler.IsEmpty());
    end;
}

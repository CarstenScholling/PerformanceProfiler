codeunit 98990 "Performance Profiler Mgt."
{
    SingleInstance = true;

    var
        EtwPerformanceProfiler: DotNet EtwPerformanceProfiler;
        RunningProfilerCode: Code[20];
        ProfilerSessionXIsAlreadyRunningErr: Label 'Profiler Session %1 is already running.', Comment = '%1 = Profiler Code';
        NoProfilerSessionIsRunningErr: Label 'No Profiler Session is running.';

    local procedure EnsurePerformanceProfiler()
    begin
        if IsNull(EtwPerformanceProfiler) then begin
            EtwPerformanceProfiler := EtwPerformanceProfiler.EtwPerformanceProfiler();
        end;
    end;

    procedure IsRunning(): Boolean
    begin
        exit(RunningProfilerCode <> '');
    end;

    local procedure EnsureRunningPerformanceProfiler()
    begin
        if not IsRunning() then begin
            Error(NoProfilerSessionIsRunningErr);
        end;
    end;

    procedure Start(performanceProfiler: Record "Performance Profiler")
    begin
        Start(performanceProfiler.Code, performanceProfiler."Session Id", performanceProfiler.Threshold);
    end;

    procedure Start(profilerCode: Code[20]; sessionId: Integer; threshold: Integer)
    begin
        if IsRunning() then begin
            Error(ProfilerSessionXIsAlreadyRunningErr, RunningProfilerCode);
        end;

        RunningProfilerCode := profilerCode;
        EnsurePerformanceProfiler();
        EtwPerformanceProfiler.Start(sessionId, threshold);
    end;

    procedure Start(profilerCode: Code[20]; sessionId: Integer)
    begin
        Start(profilerCode, sessionId, 0);
    end;

    procedure Stop()
    var
        performanceProfilerLine: Record "Performance Profiler Line";
    begin
        EnsurePerformanceProfiler();
        EnsureRunningPerformanceProfiler();
        EtwPerformanceProfiler.Stop();

        TransferAnalysis(RunningProfilerCode, performanceProfilerLine);
        Clear(RunningProfilerCode);
    end;

    local procedure TransferAnalysis(profilerCode: Code[20]; var performanceProfilerLine: Record "Performance Profiler Line")
    var
        appInfo: ModuleInfo;
        entryNo: Integer;
    begin
        performanceProfilerLine.SetRange("Performance Profiler Code", profilerCode);
        performanceProfilerLine.DeleteAll();
        entryNo := 0;

        while EtwPerformanceProfiler.CallTreeMoveNext() do begin
            entryNo += 1;
            performanceProfilerLine.Init();

            performanceProfilerLine."Performance Profiler Code" := profilerCode;
            performanceProfilerLine."Entry No." := entryNo;
            performanceProfilerLine.Identation := EtwPerformanceProfiler.CurrentIndentation;
            performanceProfilerLine."Session Id" := EtwPerformanceProfiler.CurrentSessionId;
            performanceProfilerLine.Tenant := EtwPerformanceProfiler.CurrentTenant;
            performanceProfilerLine."User Name" := EtwPerformanceProfiler.CurrentUserName;

            if not Evaluate(performanceProfilerLine."App Id", EtwPerformanceProfiler.CurrentAppId) then begin
                Clear(performanceProfilerLine."App Id");
            end;

            if not IsNullGuid(performanceProfilerLine."App Id") then begin
                NavApp.GetModuleInfo(performanceProfilerLine."App Id", appInfo);
                performanceProfilerLine."App Name" := CopyStr(appInfo.Name, 1, MaxStrLen(performanceProfilerLine."App Name"));
            end;

            performanceProfilerLine."Object Type" := performanceProfilerLine."Object Type"::Sql;
            performanceProfilerLine."Object Id" := EtwPerformanceProfiler.CurrentOwningObjectId;
            performanceProfilerLine."Object Type Name" := EtwPerformanceProfiler.CurrentOwningObjectTypeName;

            if EtwPerformanceProfiler.CurrentOwningObjectTypeName <> '' then begin
                if not Evaluate(performanceProfilerLine."Object Type", EtwPerformanceProfiler.CurrentOwningObjectTypeName) then begin
                    Clear(performanceProfilerLine."Object Type");
                end;
            end;

            performanceProfilerLine."Statement Line No." := EtwPerformanceProfiler.CurrentLineNumber;

            performanceProfilerLine.SetStatement(EtwPerformanceProfiler.CurrentStatement);

            performanceProfilerLine."Duration (ms)" := EtwPerformanceProfiler.CurrentDurationMs;
            performanceProfilerLine."Hit Count" := EtwPerformanceProfiler.CurrentHitCount;

            performanceProfilerLine."Last Active (ms)" := EtwPerformanceProfiler.CurrentLastActiveMs;
            performanceProfilerLine."Min. Duration" := EtwPerformanceProfiler.CurrentMinDurationMs;
            performanceProfilerLine."Max. Duration" := EtwPerformanceProfiler.CurrentMaxDurationMs;

            performanceProfilerLine.Insert();
        end;
    end;

    procedure AnalyzeEtlFile(profilerCode: Code[20])
    begin
        AnalyzeEtlFile(profilerCode, 0);
    end;

    procedure AnalyzeEtlFile(profilerCode: Code[20]; threshold: Integer)
    var
        fileMgt: Codeunit "File Management";
        etlTempBlob: Codeunit "Temp Blob";
        serverFilePath: Text;
        filename: Text;
        selectEtlFileTxt: Label 'Select ETL file...';
        etlExtensionTxt: Label 'etl';
        etlFileTypeTxt: Label 'ETL File (*.etl)|*.etl';
    begin
        filename := fileMgt.BLOBImportWithFilter(etlTempBlob, selectEtlFileTxt, '', '', etlFileTypeTxt);

        if filename <> '' then begin
            serverFilePath := fileMgt.ServerTempFileName(etlExtensionTxt);
            fileMgt.BLOBExportToServerFile(etlTempBlob, serverFilePath);

            AnalyzeEtlFile(profilerCode, serverFilePath, threshold);

            fileMgt.DeleteServerFile(serverFilePath);
        end;
    end;

    procedure AnalyzeEtlFile(profilerCode: Code[20]; filePath: Text)
    begin
        AnalyzeEtlFile(profilerCode, filePath, 0);
    end;

    procedure AnalyzeEtlFile(profilerCode: Code[20]; filePath: Text; threshold: Integer)
    var
        performanceProfilerLine: Record "Performance Profiler Line";
        fileMgt: Codeunit "File Management";
    begin
        if fileMgt.ServerFileExists(filePath) then begin
            EnsurePerformanceProfiler();
            EtwPerformanceProfiler.AnalyzeETLFile(filePath, threshold);
            TransferAnalysis(profilerCode, performanceProfilerLine);
        end;
    end;
}

﻿// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System.Collections.Generic;
using NLog;
using PostProcessor.IncomeDataParser;

namespace PostProcessor.CodeGenerator
{
    public class Generator : GeneratorBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public Queue<string> GenerateMillProgramm(Queue<Command> instructionsSource)
        {
            _logger.Info("Start generate mill programm");

            var millProgramm = new Queue<string>();
            var currentline = StartLine;

            millProgramm.Enqueue(ProgramStartMessage);

            _logger.Info("Start message enqueued");

            foreach (var instruction in instructionsSource)
            {
                var operationSettings = instruction.Settings;

                switch (instruction.Name)
                {
                    case CamOperations.OperationStart:
                        EnqueueOperationHeader(millProgramm, operationHeader: operationSettings[0]);
                        break;

                    case CamOperations.OperationEnd:
                        EnqueueOperationFooter(millProgramm, currentline, operationName: operationSettings[0]);
                        currentline++;
                        break;

                    case CamOperations.SetFeedRate:
                        EnqueueSetFeedRate(millProgramm, currentline, feedRate: operationSettings[1]);
                        currentline++;
                        break;

                    case CamOperations.RapidMove:
                        EnqueueRapidMoveOn(millProgramm, currentline);
                        currentline++;
                        break;

                    case CamOperations.MillMove:
                        currentline = EnqueueMillMove(millProgramm, currentline, operationSettings);
                        break;

                    case CamOperations.Cycle:
                        SetCycleDrillSettings(operationSettings);
                        break;

                    case CamOperations.Circle:
                        SetCircleMoveSettings(operationSettings);
                        break;
                    default:
                        _logger.Warn("Instruction {0} was not opperated", instruction.Name);
                        break;
                }
            }

            _logger.Info("Finish generate mill programm");

            return millProgramm;
        }
    }
}

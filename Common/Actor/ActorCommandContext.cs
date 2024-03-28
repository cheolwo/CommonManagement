﻿using Common.Actor.Builder;
using FluentValidation.Results;

namespace FrontCommon.Actor
{
    public class ActorCommandContext : ActorContext
    {
        protected readonly DtoCommandBuilder dtoCommandBuilder = new();
        protected ActorCommandContext()
        {
            OnModelCreating(dtoCommandBuilder);
        }

        protected virtual void OnModelCreating(DtoCommandBuilder dtoBuilder) 
        {
           
        }
        public DtoTypeCommandBuilder<TDto> Set<TDto>() where TDto : class
        {
            return dtoCommandBuilder.Set<TDto>();
        }
    }
}

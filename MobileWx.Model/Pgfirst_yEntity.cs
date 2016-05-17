using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{
  public  class Pgfirst_yEntity
    {
      public List<Pgfirst_yEntity2> root { get; set; }
    }
  public class Pgfirst_yEntity2
  {
      public List<Pgfirst_yEntity3> spider { get; set; }
      public List<Pgfirst_yEntity3> top { get; set; } 
      public List<Pgfirst_yEntity5> stock_tech { get; set; }
      public List<Pgfirst_yEntity5> stock_texture { get; set; } 
  }
  public class Pgfirst_yEntity3
  {
      public List<Pgfirst_yEntity4> data { get; set; }
  }
  public class Pgfirst_yEntity4
  {
    public string BBasic {get;set;}
    public string BIndustry {get;set;}
    public string  BMajor{get;set;}
    public string BMsg {get;set;}
    public string  BTech{get;set;}
    public string BTechBlock {get;set;}
    public string  SBasic{get;set;}
    public string  SIndustry{get;set;}
    public string  SMajor{get;set;}
    public string  SMsg{get;set;}
    public string  STech{get;set;}
    public string  STechBlock{get;set;}
    public string  R{get;set;}
    public string  T{get;set;}
    public string K { get; set; }
    public string  Con{get;set;}
    public string  HowToDo{get;set;}
    public string  IndustryType{get;set;}
    public string  Level{get;set;}
    public string  Summarize{get;set;}
    public string  TotalScore{get;set;}



  }
  public class Pgfirst_yEntity5
  {
      public string title { get; set; }
      public List<Pgfirst_yEntity3> group { get; set; }
      
  }

}

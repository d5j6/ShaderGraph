using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    public abstract class Function1Input : AbstractMaterialNode, IGeneratesBodyCode
    {
        public override bool hasPreview
        {
            get { return true; }
        }

        protected Function1Input(IGraph owner)
            : base(owner)
        {
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(GetInputSlot());
            AddSlot(GetOutputSlot());
            RemoveSlotsNameNotMatching(validSlots);
        }
   
        protected string[] validSlots
        {
            get { return new[] {GetInputSlotName(), GetOutputSlotName()}; }
        }

        protected virtual MaterialSlot GetInputSlot()
        {
            return new MaterialSlot(GetInputSlotName(), GetInputSlotName(), SlotType.Input, 0, SlotValueType.Dynamic, Vector4.zero);
        }

        protected virtual MaterialSlot GetOutputSlot()
        {
            return new MaterialSlot(GetOutputSlotName(), GetOutputSlotName(), SlotType.Output, 0, SlotValueType.Dynamic, Vector4.zero);
        }

        protected virtual string GetInputSlotName() {return "Input"; }
        protected virtual string GetOutputSlotName() {return "Output"; }

        protected abstract string GetFunctionName();

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            var outputSlot = FindMaterialOutputSlot(GetOutputSlotName());
            var inputSlot = FindMaterialInputSlot(GetInputSlotName());

            if (inputSlot == null || outputSlot == null)
            {
                Debug.LogError("Invalid slot configuration on node: " + name);
                return;
            }

            var inputValue = GetSlotValue(inputSlot, generationMode);
            visitor.AddShaderChunk(precision + outputDimension + " " + GetOutputVariableNameForSlot(outputSlot) + " = " + GetFunctionCallBody(inputValue) + ";", true);
        }

        protected virtual string GetFunctionCallBody(string inputValue)
        {
            return GetFunctionName() + " (" + inputValue + ")";
        }

        public string outputDimension
        {
            get { return ConvertConcreteSlotValueTypeToString(FindMaterialOutputSlot(GetOutputSlotName()).concreteValueType); }
        }
        public string inputDimension
        {
            get { return ConvertConcreteSlotValueTypeToString(FindMaterialInputSlot(GetInputSlotName()).concreteValueType); }
        }
    }
}
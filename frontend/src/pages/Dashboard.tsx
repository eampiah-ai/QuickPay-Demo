import InvoiceList from "../components/InvoiceList";
import Stats from "@/components/Stats";

export default function Dashboard() {
  return (
    <div className="flex flex-col p-5">
      <Stats />
      <InvoiceList />
    </div>
  );
}

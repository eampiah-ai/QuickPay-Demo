import { useQuery } from "@tanstack/react-query";
import axios, { type AxiosResponse } from "axios";
import { decimalFormatter } from "./InvoiceList";

interface DashCardProps {
  title: string;
  value: number;
  commentLine1?: string;
  isMonetaryValue?: boolean;
}

const fetchStats = async () =>
  await axios
    .get<any, AxiosResponse<DashCardProps[]>>("http://localhost:5250/api/stats")
    .then((response) => response.data);

export default function Stats() {
  const { data, status } = useQuery({
    queryKey: ["stats"],
    queryFn: fetchStats,
  });

  const makeDashCard = (card: DashCardProps) => {
    return (
      <div
        key={`stats-card-${card.title}`}
        className="flex flex-col border border-black border-solid rounded-md p-5 m-5 min-w-xs max-w-xs"
      >
        <p className="text-sm">{card.title}</p>
        <h1 className="text-[2em]">
          {card.isMonetaryValue
            ? `$${decimalFormatter.format(card.value / 100)}`
            : card.value}
        </h1>
        <p className="text-sm">{card.commentLine1}</p>
      </div>
    );
  };

  if (status !== "success") {
    return <div>Loading stats...</div>;
  }
  return (
    <div>
      <h1 className="text-[3em] p-5">Statistics</h1>
      <div className="flex flex-row flex-wrap">{data?.map(makeDashCard)}</div>
    </div>
  );
}
